using System.Threading.Tasks;
using AutoMapper;
using Goblin.Application.Core;
using Goblin.Application.Core.Abstractions;
using Goblin.Application.Core.Results.Failed;
using Goblin.Application.Telegram.Converters;
using Goblin.Application.Telegram.Models;
using Goblin.DataAccess;
using Goblin.Domain.Entities;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace Goblin.Application.Telegram
{
    public class TelegramCallbackHandler
    {
        private readonly TelegramBotClient _botClient;
        private readonly CommandsService _commandsService;
        private readonly BotDbContext _context;
        private readonly IMapper _mapper;

        public TelegramCallbackHandler(BotDbContext context, TelegramBotClient botClient, CommandsService commandsService, IMapper mapper)
        {
            _context = context;
            _botClient = botClient;
            _commandsService = commandsService;
            _mapper = mapper;
        }

        public async Task Handle(Update update)
        {
            if(update.Type == UpdateType.Message)
            {
                var msg = _mapper.Map<TelegramMessage>(update.Message);
                await HandleMessageEvent(msg);
            }
            else if(update.Type == UpdateType.CallbackQuery)
            {
                await HandleCallback(update.CallbackQuery);
            }
        }

        private async Task HandleMessageEvent(TelegramMessage message)
        {
            var user = await GetBotUser(message);

            var result = await _commandsService.ExecuteCommand<TgBotUser>(message, user);

            if(!result.IsSuccessful)
            {
                if(result is CommandNotFoundResult && !user.IsErrorsEnabled)
                {
                    // если команда не найдена, и у пользователя отключены ошибки
                    return;
                }

                await _botClient.SendTextMessageAsync(message.MessageChatId, result.ToString());
            }
            else
            {
                await _botClient.SendTextMessageAsync(message.MessageChatId, result.Message,
                                                      replyMarkup: KeyboardConverter.FromCoreToTg(result.Keyboard));
            }
        }

        private async Task<TgBotUser> GetBotUser(IMessage message)
        {
            var user = await _context.TgBotUsers
                                     .FindAsync(message.MessageUserId);
            if(user is null)
            {
                user = (await _context.AddAsync(new TgBotUser(message.MessageUserId))).Entity;
                await _context.SaveChangesAsync();
            }

            return user;
        }

        private async Task HandleCallback(CallbackQuery query)
        {
            var msg = _mapper.Map<TelegramCallbackMessage>(query);
            var user = await GetBotUser(msg);
            var result = await _commandsService.ExecuteCommand<TgBotUser>(msg, user);

            if(result is CommandNotFoundResult && !user.IsErrorsEnabled)
            {
                // если команда не найдена, и у пользователя отключены ошибки
                return;
            }

            await _botClient.AnswerCallbackQueryAsync(query.Id);
            await _botClient.EditMessageTextAsync(new ChatId(query.From.Id), query.Message.MessageId, result.Message);
            if(result.Keyboard.IsInline)
            {
                await _botClient.EditMessageReplyMarkupAsync(new ChatId(query.From.Id), query.Message.MessageId,
                                                             KeyboardConverter.FromCoreToTg(result.Keyboard) as InlineKeyboardMarkup);
            }
        }
    }
}