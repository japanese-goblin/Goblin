using Goblin.Application.Core;
using Goblin.Application.Core.Abstractions;
using Goblin.Application.Core.Options;
using Goblin.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Quartz;

namespace Goblin.BackgroundJobs.Jobs;

public class SendSchedulesJob(
    BotDbContext db,
    IScheduleService scheduleService,
    IEnumerable<ISender> senders,
    IOptions<MailingOptions> mailingOptions,
    ILogger<SendSchedulesJob> logger) : IJob
{
    public static readonly JobKey JobKey = new("V1", "send-schedules");

    private readonly MailingOptions _mailingOptions = mailingOptions.Value;

    public async Task Execute(IJobExecutionContext context)
    {
        if (_mailingOptions.IsVacations)
        {
            return;
        }

        var consumersGroup = db.BotUsers.AsNoTracking()
            .Where(p => p.HasScheduleSubscription && p.NarfuGroup.HasValue)
            .ToArray()
            .GroupBy(p => p.ConsumerType);
        foreach (var consumerGroup in consumersGroup)
        {
            var sender = senders.First(p => p.ConsumerType == consumerGroup.Key);
            var groupedByGroup = consumerGroup.GroupBy(p => p.NarfuGroup);
            foreach (var group in groupedByGroup)
            {
                if (!group.Key.HasValue)
                {
                    continue;
                }

                var result = await scheduleService.GetSchedule(group.Key.Value, DateTime.Today);

                foreach (var chunk in group.Chunk(Defaults.ChunkLimit))
                {
                    try
                    {
                        var ids = chunk.Select(p => p.Id).ToList();
                        await sender.SendToMany(ids, result.Message);
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "Ошибка при отправке ежедневной погоды");
                    }

                    await Task.Delay(Defaults.DelayBetweenSends);
                }
            }
        }
    }
}