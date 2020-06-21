using System;
using System.Linq;
using System.Threading.Tasks;
using Goblin.Application.Core.Extensions;
using Goblin.Application.Core.Services;
using Goblin.Application.Vk.Extensions;
using Goblin.DataAccess;
using Serilog;
using VkNet.Abstractions;
using VkNet.Model.RequestParams;

namespace Goblin.Application.Vk.Hangfire
{
    public class ScheduleTask
    {
        private readonly BotDbContext _db;
        private readonly ILogger _logger;
        private readonly ScheduleService _scheduleService;
        private readonly IVkApi _vkApi;

        public ScheduleTask(BotDbContext db, IVkApi vkApi, ScheduleService scheduleService)
        {
            _db = db;
            _vkApi = vkApi;
            _scheduleService = scheduleService;
            _logger = Log.ForContext<ScheduleTask>();
        }

        public async Task SendSchedule()
        {
            var grouped = _db.VkBotUsers.Where(x => x.HasScheduleSubscription)
                             .ToArray()
                             .GroupBy(x => x.NarfuGroup);
            foreach(var group in grouped)
            {
                foreach(var chunk in group.Chunk(Defaults.ChunkLimit))
                {
                    try
                    {
                        var ids = chunk.Select(x => x.Id);
                        var result = await _scheduleService.GetSchedule(group.Key, DateTime.Today);
                        await _vkApi.Messages.SendToUserIdsWithRandomId(new MessagesSendParams
                        {
                            UserIds = ids,
                            Message = result.Message
                        });

                        await Task.Delay(TimeSpan.FromSeconds(1.5));
                    }
                    catch(Exception ex)
                    {
                        _logger.Error(ex, "Ошибка при отправке расписания");
                    }
                }
            }
        }
    }
}