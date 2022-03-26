using System.Threading.Tasks;
using Goblin.Application.Core;
using Goblin.Domain;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace Goblin.Application.Telegram;

public class TelegramSender : ISender
{
    public ConsumerType ConsumerType => ConsumerType.Telegram;

    private readonly TelegramBotClient _botClient;

    public TelegramSender(TelegramBotClient botClient)
    {
        _botClient = botClient;
    }

    public Task Send(long chatId, string message)
    {
        return _botClient.SendTextMessageAsync(chatId, message, ParseMode.Markdown);
    }
}