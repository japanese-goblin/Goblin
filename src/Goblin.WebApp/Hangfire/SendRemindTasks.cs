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
    public class SendRemindTasks
    {
        private readonly TelegramBotClient _botClient;
        private readonly BotDbContext _db;
        private readonly IVkApi _vkApi;

        public SendRemindTasks(IVkApi vkApi, BotDbContext db, TelegramBotClient botClient)
        {
            _vkApi = vkApi;
            _db = db;
            _botClient = botClient;
        }

        public async Task SendRemindEveryMinute()
        {
            var currentTime = DateTime.Now;
            var reminds =
                    _db.Reminds
                       .Where(x => x.Date - currentTime <= TimeSpan.FromMinutes(1))
                       .ToArray();

            await SendRemindsFromArray(reminds);
        }

        public async Task SendOldReminds()
        {
            var currentTime = DateTime.Now;
            var reminds = _db.Reminds.Where(x => x.Date < currentTime).ToArray();
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