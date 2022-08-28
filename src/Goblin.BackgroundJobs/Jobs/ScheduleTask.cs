using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Goblin.Application.Core;
using Goblin.Application.Core.Abstractions;
using Goblin.Application.Core.Options;
using Goblin.DataAccess;
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
        if(_mailingOptions.IsVacations)
        {
            return;
        }

        var consumersGroup = _db.BotUsers.AsNoTracking()
                                .Where(x => x.HasScheduleSubscription)
                                .ToArray()
                                .GroupBy(x => x.ConsumerType);
        foreach(var consumerGroup in consumersGroup)
        {
            var sender = _senders.FirstOrDefault(x => x.ConsumerType == consumerGroup.Key);
            var groupedByGroup = consumerGroup.GroupBy(x => x.NarfuGroup);
            foreach(var group in groupedByGroup)
            {
                var result = await _scheduleService.GetSchedule(group.Key, DateTime.Today);

                foreach(var chunk in group.Chunk(Defaults.ChunkLimit))
                {
                    try
                    {
                        var ids = chunk.Select(x => x.Id);
                        await sender.SendToMany(ids, result.Message);
                    }
                    catch(Exception ex)
                    {
                        _logger.Error(ex, "Ошибка при отправке ежедневной погоды");
                    }

                    await Task.Delay(TimeSpan.FromSeconds(1.5));
                }
            }
        }
    }
}