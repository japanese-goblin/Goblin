using Goblin.Domain;
using Telegram.Bot.Types;
using Message = Goblin.Application.Core.Models.Message;
using TelegramMessage = Telegram.Bot.Types.Message;

namespace Goblin.Application.Telegram.Converters;

public static class MessageConverter
{
    public static Message MapToBotMessage(this TelegramMessage message)
    {
        return new Message
        {
            UserId = message.From?.Id ?? 0,
            ConsumerType = ConsumerType.Telegram,
            Text = message.Text,
            ChatId = message.Chat.Id,
            UserTag = $"@{message.Chat.Username} (`{message.From?.Id}`)"
        };
    }

    public static Message MapToBotMessage(this CallbackQuery callbackQuery)
    {
        return new Message
        {
            Text = callbackQuery.Message?.Text,
            Payload = callbackQuery.Data,
            UserId = callbackQuery.From.Id,
            ConsumerType = ConsumerType.Telegram,
            ChatId = callbackQuery.From.Id
        };
    }
}