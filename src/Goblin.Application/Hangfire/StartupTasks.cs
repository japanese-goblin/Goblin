using System;
using System.Linq;
using System.Threading.Tasks;
using Goblin.Application.Extensions;
using Goblin.DataAccess;
using VkNet.Abstractions;
using VkNet.Model.RequestParams;

namespace Goblin.Application.Hangfire
{
    public class StartupTasks
    {
        private readonly BotDbContext _db;
        private readonly IVkApi _vkApi;

        public StartupTasks(BotDbContext db, IVkApi vkApi)
        {
            _db = db;
            _vkApi = vkApi;
        }

        public async Task SendOldReminds()
        {
            var reminds = _db.Reminds.Where(x => x.Date < DateTime.Now);

            foreach (var remind in reminds)
            {
                await _vkApi.Messages.SendWithRandomId(new MessagesSendParams
                {
                    PeerId = remind.BotUserId,
                    Message = $"Напоминаю:\n{remind.Text}"
                });

                _db.Reminds.Remove(remind);
            }

            await _db.SaveChangesAsync();
        }
    }
}