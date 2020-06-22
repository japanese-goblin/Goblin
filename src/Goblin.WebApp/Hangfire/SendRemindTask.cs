using System;
using System.Linq;
using System.Threading.Tasks;
using Goblin.Application.Vk.Extensions;
using Goblin.DataAccess;
using Goblin.Domain;
using Goblin.Domain.Entities;
using Telegram.Bot;
using VkNet.Abstractions;
using VkNet.Model.RequestParams;

namespace Goblin.WebApp.Hangfire
{
    public class SendRemindTask
    {
        private const string DateTimeFormat = "dd.MM.yyyy HH:mm";
        private readonly TelegramBotClient _botClient;
        private readonly BotDbContext _db;
        private readonly IVkApi _vkApi;

        public SendRemindTask(IVkApi vkApi, BotDbContext db, TelegramBotClient botClient)
        {
            _vkApi = vkApi;
            _db = db;
            _botClient = botClient;
        }

        public async Task SendRemindEveryMinute()
        {
            var reminds =
                    _db.Reminds
                       .ToArray()
                       .Where(x => x.Date.ToString(DateTimeFormat) ==
                                   DateTime.Now.ToString(DateTimeFormat))
                       .ToArray(); //TODO: fix it

            await SendRemindsFromArray(reminds);
        }

        public async Task SendOldReminds()
        {
            var reminds = _db.Reminds.Where(x => x.Date < DateTime.Now).ToArray();
            await SendRemindsFromArray(reminds);
        }

        private async Task SendRemindsFromArray(Remind[] reminds)
        {
            if(!reminds.Any())
            {
                return;
            }

            foreach(var remind in reminds)
            {
                var message = $"Напоминаю:\n{remind.Text}";
                if(remind.ConsumerType == ConsumerType.Vkontakte)
                {
                    await SendToVk(message, remind.ChatId);
                }
                else if(remind.ConsumerType == ConsumerType.Telegram)
                {
                    await SendToTelegram(message, remind.ChatId);
                }

                _db.Reminds.Remove(remind);
            }

            if(_db.ChangeTracker.HasChanges())
            {
                await _db.SaveChangesAsync();
            }
        }

        private async Task SendToVk(string message, long chatId)
        {
            await _vkApi.Messages.SendWithRandomId(new MessagesSendParams
            {
                Message = message,
                PeerId = chatId
            });
        }

        private async Task SendToTelegram(string message, long chatId)
        {
            await _botClient.SendTextMessageAsync(chatId, message);
        }
    }
}