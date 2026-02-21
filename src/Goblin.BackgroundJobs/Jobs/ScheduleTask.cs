using Goblin.Application.Core;
using Goblin.Application.Core.Abstractions;
using Goblin.Application.Core.Options;
using Goblin.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Goblin.BackgroundJobs.Jobs;

public class ScheduleTask(
        BotDbContext db,
        IScheduleService scheduleService,
        IEnumerable<ISender> senders,
        IOptions<MailingOptions> mailingOptions,
        ILogger<ScheduleTask> logger)
{
    private readonly MailingOptions _mailingOptions = mailingOptions.Value;

    private static readonly TimeSpan DelayBetweenSends = TimeSpan.FromSeconds(1.5);

    public async Task Execute()
    {
        if(_mailingOptions.IsVacations)
        {
            return;
        }

        var consumersGroup = db.BotUsers.AsNoTracking()
                                .Where(x => x.HasScheduleSubscription && x.NarfuGroup.HasValue)
                                .ToArray()
                                .GroupBy(x => x.ConsumerType);
        foreach(var consumerGroup in consumersGroup)
        {
            var sender = senders.First(x => x.ConsumerType == consumerGroup.Key);
            var groupedByGroup = consumerGroup.GroupBy(x => x.NarfuGroup);
            foreach(var group in groupedByGroup)
            {
                if(!group.Key.HasValue)
                {
                    continue;
                }

                var result = await scheduleService.GetSchedule(group.Key.Value, DateTime.Today);

                foreach(var chunk in group.Chunk(Defaults.ChunkLimit))
                {
                    try
                    {
                        var ids = chunk.Select(x => x.Id).ToList();
                        await sender.SendToMany(ids, result.Message);
                    }
                    catch(Exception ex)
                    {
                        logger.LogError(ex, "Ошибка при отправке ежедневной погоды");
                    }

                    await Task.Delay(DelayBetweenSends);
                }
            }
        }
    }
}