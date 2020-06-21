using System;
using System.Linq;
using System.Threading.Tasks;
using Goblin.Application.Core.Commands.Keyboard;
using Goblin.Application.Core.Results.Failed;
using Goblin.Application.Core.Results.Success;
using Goblin.Application.Vk.Extensions;
using Goblin.DataAccess;
using Microsoft.EntityFrameworkCore;
using Serilog;
using VkNet.Abstractions;
using VkNet.Model.RequestParams;

namespace Goblin.Application.Vk.Hangfire
{
    public class ScheduleTask
    {
        private readonly ScheduleCommand _command;
        private readonly BotDbContext _db;
        private readonly ILogger _logger;
        private readonly IVkApi _vkApi;

        public ScheduleTask(BotDbContext db, IVkApi vkApi, ScheduleCommand command)
        {
            _db = db;
            _vkApi = vkApi;
            _command = command;
            _logger = Log.ForContext<ScheduleTask>();
        }

        public async Task SendSchedule()
        {
            var grouped = _db.BotUsers.Include(x => x.SubscribeInfo)
                             .Where(x => x.SubscribeInfo.IsSchedule)
                             .ToArray()
                             .GroupBy(x => x.NarfuGroup);
            foreach(var group in grouped)
            {
                foreach(var chunk in group.Chunk(Defaults.ChunkLimit))
                {
                    try
                    {
                        var ids = chunk.Select(x => x.VkId);
                        var schedule = await _command.GetSchedule(group.Key, DateTime.Today);
                        if(schedule is FailedResult failed)
                        {
                            await _vkApi.Messages.SendErrorToUserIds(failed.Message, ids);
                        }
                        else
                        {
                            var success = schedule as SuccessfulResult;
                            await _vkApi.Messages.SendToUserIdsWithRandomId(new MessagesSendParams
                            {
                                UserIds = ids,
                                Message = success.Message
                            });
                        }

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