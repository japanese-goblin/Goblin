using System;
using System.Linq;
using System.Threading.Tasks;
using Flurl.Http;
using Goblin.Application.Extensions;
using Goblin.DataAccess;
using Goblin.Narfu;
using Microsoft.EntityFrameworkCore;
using Serilog;
using VkNet.Abstractions;
using VkNet.Model.RequestParams;

namespace Goblin.Application.Hangfire
{
    public class ScheduleTask
    {
        private readonly BotDbContext _db;
        private readonly NarfuApi _narfuApi;
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
                        await _vkApi.Messages.SendToUserIdsWithRandomId(new MessagesSendParams
                        {
                            Message = schedule.ToString(),
                            UserIds = ids
                        });
                    }
                    catch(FlurlHttpException ex)
                    {
                        Log.Error("ruz.narfu.ru недоступен (http code - {0}", ex.Call.HttpStatus);
                        var msg = $"Невозможно получить расписание с сайта (код ошибки - {ex.Call.HttpStatus}).";
                        await _vkApi.Messages.SendErrorToUserIds(msg, ids);
                    }
                    catch(Exception ex)
                    {
                        Log.Fatal(ex, "Ошибка при получении расписания");
                        var msg = "Непредвиденнная ошибка при получении расписания с сайта.";
                        await _vkApi.Messages.SendErrorToUserIds(msg, ids);
                    }

                    await Task.Delay(Defaults.ExtraDelay);
                }
            }
        }
    }
}