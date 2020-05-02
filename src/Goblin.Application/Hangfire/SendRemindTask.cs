using System;
using System.Linq;
using System.Threading.Tasks;
using Goblin.Application.Extensions;
using Goblin.DataAccess;
using VkNet.Abstractions;
using VkNet.Model.RequestParams;

namespace Goblin.Application.Hangfire
{
    public class SendRemindTask
    {
        private readonly BotDbContext _db;
        private readonly IVkApi _vkApi;

        public SendRemindTask(IVkApi vkApi, BotDbContext db)
        {
            _vkApi = vkApi;
            _db = db;
        }

        public async Task SendRemind()
        {
            var reminds =
                    _db.Reminds
                       .AsEnumerable()
                       .Where(x => x.Date.ToString("dd.MM.yyyy HH:mm") ==
                                   DateTime.Now.ToString("dd.MM.yyyy HH:mm"))
                       .ToArray(); //TODO: fix it

            if(!reminds.Any())
            {
                return;
            }

            foreach(var remind in reminds)
            {
                await _vkApi.Messages.SendWithRandomId(new MessagesSendParams
                {
                    Message = $"Напоминаю:\n{remind.Text}",
                    PeerId = remind.BotUserId
                });
                _db.Reminds.Remove(remind);
            }

            if(_db.ChangeTracker.HasChanges())
            {
                await _db.SaveChangesAsync();
            }
        }
    }
}