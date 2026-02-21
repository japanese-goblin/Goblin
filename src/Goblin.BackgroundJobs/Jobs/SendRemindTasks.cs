using Goblin.Application.Core;
using Goblin.DataAccess;
using Goblin.Domain.Entities;

namespace Goblin.BackgroundJobs.Jobs;

public class SendRemindTasks(BotDbContext db, IEnumerable<ISender> senders)
{
    public async Task SendRemindEveryMinute()
    {
        var currentTime = DateTimeOffset.UtcNow;
        var reminds =
                db.Reminds
                   .Where(x => x.Date - currentTime <= TimeSpan.FromMinutes(1))
                   .ToArray();

        await SendRemindsFromArray(reminds);
    }

    public async Task SendOldRemindsOnStartup()
    {
        var currentTime = DateTimeOffset.UtcNow;
        var reminds = db.Reminds.Where(x => x.Date < currentTime).ToArray();
        await SendRemindsFromArray(reminds);
    }

    private async Task SendRemindsFromArray(Remind[] reminds)
    {
        if(reminds.Length == 0)
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
            await db.SaveChangesAsync();
        }
    }
}