using System;
using System.Linq;
using System.Threading.Tasks;
using Goblin.Application.Core.Extensions;
using Goblin.Application.Vk;
using Goblin.Application.Vk.Extensions;
using Goblin.Application.Vk.Hangfire;
using Goblin.DataAccess;
using Goblin.Domain;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Telegram.Bot;
using VkNet.Abstractions;
using VkNet.Model.RequestParams;

namespace Goblin.WebApp.Hangfire
{
    public class SendToUsersTasks
    {
        private readonly IVkApi _vkApi;
        private readonly BotDbContext _db;
        private readonly TelegramBotClient _botClient;
        private readonly ILogger _logger;

        public SendToUsersTasks(IVkApi vkApi, BotDbContext db, TelegramBotClient botClient)
        {
            _vkApi = vkApi;
            _db = db;
            _botClient = botClient;
            _logger = Log.ForContext<SendToUsersTasks>();
        }

        public async Task SendToAll(string text, string[] attachments, ConsumerType type)
        {
            if(type == ConsumerType.Vkontakte)
            {
                await SendToAllVk(text, attachments);
                return;
            }

            if(type == ConsumerType.Telegram)
            {
                await SendToAllTelegram(text, attachments);
                return;
            }

            if(type == ConsumerType.AllInOne)
            {
                await SendToAllVk(text, attachments);
                await SendToAllTelegram(text, attachments);
            }
        }

        public async Task SendToId(long chatId, string text, string[] attachments, ConsumerType type)
        {
            if(type == ConsumerType.Vkontakte)
            {
                await SendToVk(chatId, text, attachments);
            }
            else if(type == ConsumerType.Telegram)
            {
                await SendToTelegram(chatId, text, attachments);
            }
        }

        private async Task SendToAllVk(string text, string[] attachs)
        {
            var chunks = _db.VkBotUsers
                            .AsNoTracking()
                            .Select(x => x.Id)
                            .AsEnumerable()
                            .Chunk(Defaults.ChunkLimit);

            var attachments = AttachmentsParser.StringsToAttachments(attachs);

            foreach(var chunk in chunks)
            {
                try
                {
                    await _vkApi.Messages.SendToUserIdsWithRandomId(new MessagesSendParams
                    {
                        Message = text,
                        UserIds = chunk,
                        Attachments = attachments
                    });
                }
                catch(Exception e)
                {
                    _logger.Warning("{0} ({1})", e.GetType(), e.Message);
                }
            }
        }

        private async Task SendToVk(long chatId, string text, string[] attachs)
        {
            var attachments = AttachmentsParser.StringsToAttachments(attachs);
            await _vkApi.Messages.SendWithRandomId(new MessagesSendParams
            {
                PeerId = chatId,
                Message = text,
                Attachments = attachments
            });
        }

        private async Task SendToAllTelegram(string text, string[] attachs)
        {
            var chunks = _db.TgBotUsers
                            .AsNoTracking()
                            .Select(x => x.Id)
                            .ToArray()
                            .Chunk(25);

            foreach(var chunk in chunks)
            {
                foreach(var id in chunk)
                {
                    await SendToTelegram(id, text, attachs);
                }

                await Task.Delay(1500);
            }
        }

        private async Task SendToTelegram(long chatId, string text, string[] attachments)
        {
            try
            {
                await _botClient.SendTextMessageAsync(chatId, text);
            }
            catch(Exception e)
            {
                _logger.Warning("{0} [1] ({2})", e.GetType(), chatId, e.Message);
            }
        }
    }
}