using System;
using System.Linq;
using System.Threading.Tasks;
using Goblin.Application.Extensions;
using Goblin.DataAccess;
using Goblin.Narfu;
using Microsoft.EntityFrameworkCore;
using VkNet.Abstractions;
using VkNet.Model.RequestParams;

namespace Goblin.Application.Hangfire
{
    public class ScheduleTask
    {
        private readonly NarfuApi _narfuApi;
        private readonly BotDbContext _db;
        private readonly IVkApi _vkApi;

        public ScheduleTask(NarfuApi narfuApi, BotDbContext db, IVkApi vkApi)
        {
            _narfuApi = narfuApi;
            _db = db;
            _vkApi = vkApi;
        }
        
        public async Task SendSchedule()
        {
            var grouped = _db.BotUsers.Include(x => x.SubscribeInfo)
                             .Where(x => x.SubscribeInfo.IsSchedule)
                             .GroupBy(x => x.NarfuGroup);
            foreach(var group in grouped)
            {
                foreach(var chunk in group.Chunk(Defaults.ChunkLimit))
                {
                    var ids = chunk.Select(x => x.VkId);
                    try
                    {
                        var schedule = await _narfuApi.Students.GetScheduleAtDate(group.Key, DateTime.Today);
                        await _vkApi.Messages.SendToUserIdsAsync(new MessagesSendParams
                        {
                            Message = schedule.ToString(),
                            UserIds = ids,
                            RandomId = new Random().Next(0, int.MaxValue)
                        });
                    }
                    catch
                    {
                        await _vkApi.Messages.SendToUserIdsAsync(new MessagesSendParams
                        {
                            Message = "Ошибка получения расписания с сайта.",
                            UserIds = ids,
                            RandomId = new Random().Next(0, int.MaxValue)
                        });
                    }
                    
                    await Task.Delay(Defaults.ExtraDelay);
                }
            }
        }
    }
}