using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Goblin.Application.Core;
using Goblin.Application.Core.Abstractions;
using Goblin.Application.Telegram.Converters;
using Goblin.DataAccess;
using Goblin.Domain;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Message = Goblin.Application.Core.Models.Message;

namespace Goblin.Application.Telegram;

public class TelegramCallbackHandler
{
    private readonly TelegramBotClient _botClient;
    private readonly CommandsService _commandsService;
    private readonly BotDbContext _context;
    private readonly ISender _sender;

    public TelegramCallbackHandler(TelegramBotClient botClient, CommandsService commandsService,
                                   IEnumerable<ISender> senders, BotDbContext context)
    {
        _botClient = botClient;
        _commandsService = commandsService;
        _context = context;
        _sender = senders.First(x => x.ConsumerType == ConsumerType.Telegram);
    }

    public async Task Handle(Update update)
    {
        if(update.Type == UpdateType.Message)
        {
            await HandleMessageEvent(update.Message.MapToBotMessage());
        }
        else if(update.Type == UpdateType.CallbackQuery)
        {
            await HandleCallback(update.CallbackQuery);
        }
        else if(update.Type == UpdateType.MyChatMember &&
                update.MyChatMember?.NewChatMember.Status == ChatMemberStatus.Kicked)
        {
            await HandleBotKick(update.MyChatMember);
        }
    }

    private async Task HandleMessageEvent(Message message)
    {
        await _commandsService.ExecuteCommand(message, OnSuccess, OnFailed);

        async Task OnSuccess(IResult res)
        {
            await _sender.Send(message.ChatId, res.Message, res.Keyboard);
        }

        async Task OnFailed(IResult res)
        {
            await _sender.Send(message.ChatId, res.Message);
        }
    }

    private async Task HandleCallback(CallbackQuery query)
    {
        await _commandsService.ExecuteCommand(query.MapToBotMessage(), OnAnyResult, OnAnyResult);

        async Task OnAnyResult(IResult res)
        {
            await _botClient.AnswerCallbackQuery(query.Id);
            await _botClient.EditMessageText(new ChatId(query.From.Id), query.Message.MessageId, res.Message);
            if(res.Keyboard.IsInline)
            {
                await _botClient.EditMessageReplyMarkup(new ChatId(query.From.Id), query.Message.MessageId,
                                                        KeyboardConverter.FromCoreToTg(res.Keyboard) as InlineKeyboardMarkup);
            }
        }
    }

    private async Task HandleBotKick(ChatMemberUpdated updateMyChatMember)
    {
        var user = await _context.BotUsers.FirstOrDefaultAsync(x => x.Id == updateMyChatMember.From.Id);
        if(user is not null)
        {
            _context.BotUsers.Remove(user);
            await _context.SaveChangesAsync();
        }
    }
}