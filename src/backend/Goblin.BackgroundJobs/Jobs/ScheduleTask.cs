using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Goblin.Application.Core;
using Goblin.Application.Core.Abstractions;
using Goblin.Application.Core.Options;
using Goblin.DataAccess;
using Goblin.Domain;
using Goblin.Domain.Abstractions;
using Goblin.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Serilog;

namespace Goblin.BackgroundJobs.Jobs;

public class ScheduleTask
{
    private readonly BotDbContext _db;
    private readonly ILogger _logger;
    private readonly IScheduleService _scheduleService;
    private readonly IEnumerable<ISender> _senders;
    private readonly MailingOptions _mailingOptions;

    public ScheduleTask(BotDbContext db,
                        IScheduleService scheduleService,
                        IEnumerable<ISender> senders,
                        IOptions<MailingOptions> mailingOptions)
    {
        _db = db;
        _scheduleService = scheduleService;
        _senders = senders;
        _mailingOptions = mailingOptions.Value;
        _logger = Log.ForContext<ScheduleTask>();
    }

    public async Task Execute()
    {
        Func<string, IEnumerable<long>, ConsumerType, Task> send = async (text, userIds, consumer) =>
        {
            var sender = _senders.FirstOrDefault(x => x.ConsumerType == consumer);
            await sender.SendToMany(userIds, text);
        };

        await SendSchedule<VkBotUser>(send);
        await SendSchedule<TgBotUser>(send);
    }

    private async Task SendSchedule<T>(Func<string, IEnumerable<long>, ConsumerType, Task> func) where T : BotUser
    {
        var grouped = _db.Set<T>()
                         .AsNoTracking()
                         .Where(x => x.HasScheduleSubscription)
                         .ToArray()
                         .GroupBy(x => x.NarfuGroup);
        foreach(var group in grouped)
        {
            var result = await _scheduleService.GetSchedule(group.Key, DateTime.Today);
            if(!result.IsSuccessful && _mailingOptions.IsVacations)
            {
                continue;
            }

            foreach(var chunk in group.Chunk(Defaults.ChunkLimit))
            {
                try
                {
                    var ids = chunk.Select(x => x.Id);
                    await func(result.Message, ids, chunk.FirstOrDefault().ConsumerType); //TODO:
                }
                catch(Exception ex)
                {
                    _logger.Error(ex, "Ошибка при отправке ежедневного расписания");
                }

                await Task.Delay(TimeSpan.FromSeconds(1.5));
            }
        }
    }
}