using Goblin.Application.Core;
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

    public TelegramCallbackHandler(
        TelegramBotClient botClient, 
        CommandsService commandsService,
        IEnumerable<ISender> senders, 
        BotDbContext context)
    {
        _botClient = botClient;
        _commandsService = commandsService;
        _context = context;
        _sender = senders.First(p => p.ConsumerType == ConsumerType.Telegram);
    }

    public async Task Handle(Update update)
    {
        if (update.Type == UpdateType.Message)
        {
            await HandleMessageEvent(update.Message.MapToBotMessage());
        }
        else if (update.Type == UpdateType.CallbackQuery)
        {
            await HandleCallback(update.CallbackQuery);
        }
        else if (update is { Type: UpdateType.MyChatMember, MyChatMember.NewChatMember.Status: ChatMemberStatus.Kicked })
        {
            await HandleBotKick(update.MyChatMember);
        }
    }

    private async Task HandleMessageEvent(Message message)
    {
        var result = await _commandsService.ExecuteAction(message, CancellationToken.None);
        await _sender.Send(message.ChatId, result.Message, result.Keyboard);
    }

    private async Task HandleCallback(CallbackQuery query)
    {
        var result = await _commandsService.ExecuteAction(query.MapToBotMessage(), CancellationToken.None);
        await _botClient.AnswerCallbackQuery(query.Id);
        await _botClient.EditMessageText(new ChatId(query.From.Id), query.Message.MessageId, result.Message);
        if (result.Keyboard?.IsInline == true)
        {
            await _botClient.EditMessageReplyMarkup(new ChatId(query.From.Id),
                query.Message.MessageId,
                KeyboardConverter.FromCoreToTg(result.Keyboard) as InlineKeyboardMarkup);
        }
    }

    private async Task HandleBotKick(ChatMemberUpdated updateMyChatMember)
    {
        var user = await _context.BotUsers.FirstOrDefaultAsync(p => p.ConsumerType == ConsumerType.Telegram &&
                                                                    p.ConsumerId == updateMyChatMember.From.Id);
        if (user is not null)
        {
            _context.BotUsers.Remove(user);
            await _context.SaveChangesAsync();
        }
    }
}
