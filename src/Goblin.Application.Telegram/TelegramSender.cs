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
using Telegram.Bot.Types.Enums;

namespace Goblin.Application.Telegram;

public class TelegramSender : ISender
{
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
        var replyMarkup = KeyboardConverter.FromCoreToTg(keyboard);
        return _botClient.SendTextMessageAsync(chatId, message, replyMarkup: replyMarkup);
    }

    public async Task SendToMany(IEnumerable<long> chatIds, string message, CoreKeyboard keyboard = null, IEnumerable<string> attachments = null)
    {
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
}