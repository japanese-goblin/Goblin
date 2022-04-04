using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Goblin.Application.Core;
using Goblin.Application.Core.Models;
using Goblin.Application.Telegram.Converters;
using Goblin.Domain;
using Serilog;
using Telegram.Bot;

namespace Goblin.Application.Telegram;

public class TelegramSender : ISender
{
    public int TextLimit => 4096;
    public ConsumerType ConsumerType => ConsumerType.Telegram;

    private readonly TelegramBotClient _botClient;
    private readonly ILogger _logger;

    public TelegramSender(TelegramBotClient botClient)
    {
        _botClient = botClient;
        _logger = Log.ForContext<TelegramSender>();
    }

    public Task Send(long chatId, string message, CoreKeyboard keyboard = null, IEnumerable<string> attachments = null)
    {
        message = TrimText(message);
        var replyMarkup = KeyboardConverter.FromCoreToTg(keyboard);
        return _botClient.SendTextMessageAsync(chatId, message, replyMarkup: replyMarkup);
    }

    public async Task SendToMany(IEnumerable<long> chatIds, string message, CoreKeyboard keyboard = null,
                                 IEnumerable<string> attachments = null)
    {
        message = TrimText(message);
        foreach(var chunk in chatIds.Chunk(25))
        {
            foreach(var id in chunk)
            {
                try
                {
                    await Send(id, message, keyboard, attachments);
                }
                catch(Exception e)
                {
                    _logger.Error(e, "Ошибка при отправке сообщения");
                }
            }

            await Task.Delay(1500);
        }
    }

    private string TrimText(string text)
    {
        const string separator = "...";
        var limit = TextLimit - separator.Length - 2;
        return $"{text[..limit]}...";
    }
}