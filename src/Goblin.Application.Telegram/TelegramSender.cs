using Goblin.Application.Core;
using Goblin.Application.Core.Models;
using Goblin.Application.Telegram.Converters;
using Goblin.Domain;
using Microsoft.Extensions.Logging;
using Telegram.Bot;

namespace Goblin.Application.Telegram;

public class TelegramSender(TelegramBotClient botClient, ILogger<TelegramSender> logger) : ISender
{
    public int TextLimit => 4096;

    public ConsumerType ConsumerType => ConsumerType.Telegram;

    public Task Send(
        long chatId,
        string message, 
        CoreKeyboard? keyboard = null, 
        IReadOnlyCollection<string>? attachments = null)
    {
        message = TrimText(message);
        var replyMarkup = KeyboardConverter.FromCoreToTg(keyboard);
        return botClient.SendMessage(chatId, message, replyMarkup: replyMarkup);
    }

    public async Task SendToMany(
        IReadOnlyCollection<long> chatIds,
        string message,
        CoreKeyboard? keyboard = null,
        IReadOnlyCollection<string>? attachments = null)
    {
        message = TrimText(message);
        foreach (var chunk in chatIds.Chunk(25))
        {
            foreach (var id in chunk)
            {
                try
                {
                    await Send(id, message, keyboard, attachments);
                }
                catch(Exception e)
                {
                    logger.LogError(e, "Ошибка при отправке сообщения {UserId}", id);
                }
            }

            await Task.Delay(1500);
        }
    }

    private string TrimText(string text)
    {
        if (text.Length < TextLimit)
        {
            return text;
        }

        const string separator = "...";
        var limit = TextLimit - separator.Length - 2;
        return $"{text[..limit]}...";
    }
}
