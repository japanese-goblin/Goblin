using Goblin.Application.Core;
using Goblin.DataAccess;
using Goblin.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Quartz;

namespace Goblin.BackgroundJobs.Jobs;

internal class SendRemindsJob(BotDbContext db, TimeProvider timeProvider, IEnumerable<ISender> senders) : IJob
{
    public static readonly JobKey JobKey = new("V1", "send-reminds");

    public async Task Execute(IJobExecutionContext context)
    {
        var currentTime = timeProvider.GetUtcNow();
        var reminds = await db.Reminds
                .Where(p => p.Date - currentTime <= TimeSpan.FromMinutes(1))
                .ToListAsync();

        await SendReminds(reminds, context.CancellationToken);
    }

    private async Task SendReminds(IReadOnlyCollection<Remind> reminds, CancellationToken cancellationToken)
    {
        if(reminds.Count == 0)
        {
            return;
        }

        foreach(var remind in reminds)
        {
            var message = $"Напоминаю:\n{remind.Text}";
            var sender = senders.First(x => x.ConsumerType == remind.ConsumerType);

            await sender.Send(remind.ChatId, message);

            db.Reminds.Remove(remind);
        }

        if(db.ChangeTracker.HasChanges())
        {
            await db.SaveChangesAsync(cancellationToken);
        }
    }
}