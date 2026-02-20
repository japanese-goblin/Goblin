using Goblin.Application.Core;
using Goblin.DataAccess;
using Goblin.Domain.Entities;

namespace Goblin.BackgroundJobs.Jobs;

public class SendRemindTasks
{
    private readonly BotDbContext _db;
    private readonly IEnumerable<ISender> _senders;

    public SendRemindTasks(BotDbContext db, IEnumerable<ISender> senders)
    {
        _db = db;
        _senders = senders;
    }

    public async Task SendRemindEveryMinute()
    {
        var currentTime = DateTimeOffset.UtcNow;
        var reminds =
                _db.Reminds
                   .Where(x => x.Date - currentTime <= TimeSpan.FromMinutes(1))
                   .ToArray();

        await SendRemindsFromArray(reminds);
    }

    public async Task SendOldRemindsOnStartup()
    {
        var currentTime = DateTimeOffset.UtcNow;
        var reminds = _db.Reminds.Where(x => x.Date < currentTime).ToArray();
        await SendRemindsFromArray(reminds);
    }

    private async Task SendRemindsFromArray(Remind[] reminds)
    {
        if(!reminds.Any())
        {
            return;
        }

        foreach(var remind in reminds)
        {
            var message = $"Напоминаю:\n{remind.Text}";
            var sender = _senders.First(x => x.ConsumerType == remind.ConsumerType);

            await sender.Send(remind.ChatId, message);

            _db.Reminds.Remove(remind);
        }

        if(_db.ChangeTracker.HasChanges())
        {
            await _db.SaveChangesAsync();
        }
    }
}