using System.Threading.Tasks;
using AutoMapper;
using Goblin.Application.Core;
using Goblin.Application.Core.Abstractions;
using Goblin.Application.Telegram.Converters;
using Goblin.Domain.Entities;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Message = Goblin.Application.Core.Models.Message;

namespace Goblin.Application.Telegram
{
    public class TelegramCallbackHandler
    {
        private readonly TelegramBotClient _botClient;
        private readonly CommandsService _commandsService;
        private readonly IMapper _mapper;

        public TelegramCallbackHandler(TelegramBotClient botClient, CommandsService commandsService,
                                       IMapper mapper)
        {
            _botClient = botClient;
            _commandsService = commandsService;
            _mapper = mapper;
        }

        public async Task Handle(Update update)
        {
            if(update.Type == UpdateType.Message)
            {
                var msg = _mapper.Map<Message>(update.Message);
                await HandleMessageEvent(msg);
            }
            else if(update.Type == UpdateType.CallbackQuery)
            {
                await HandleCallback(update.CallbackQuery);
            }
        }

        private async Task HandleMessageEvent(Message message)
        {
            await _commandsService.ExecuteCommand<TgBotUser>(message, OnSuccess, OnFailed);

            async Task OnSuccess(IResult res)
            {
                await _botClient.SendTextMessageAsync(message.ChatId, res.Message,
                                                      replyMarkup: KeyboardConverter.FromCoreToTg(res.Keyboard));
            }

            async Task OnFailed(IResult res)
            {
                await _botClient.SendTextMessageAsync(message.ChatId, res.Message);
            }
        }

        private async Task HandleCallback(CallbackQuery query)
        {
            var msg = _mapper.Map<Message>(query);
            await _commandsService.ExecuteCommand<TgBotUser>(msg, OnAnyResult, OnAnyResult);

            async Task OnAnyResult(IResult res)
            {
                await _botClient.AnswerCallbackQueryAsync(query.Id);
                await _botClient.EditMessageTextAsync(new ChatId(query.From.Id), query.Message.MessageId, res.Message);
                if(res.Keyboard.IsInline)
                {
                    await _botClient.EditMessageReplyMarkupAsync(new ChatId(query.From.Id), query.Message.MessageId,
                                                                 KeyboardConverter.FromCoreToTg(res.Keyboard) as InlineKeyboardMarkup);
                }
            }
        }
    }
}