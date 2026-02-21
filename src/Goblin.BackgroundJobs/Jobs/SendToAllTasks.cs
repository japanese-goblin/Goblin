using Goblin.Application.Core;
using Goblin.DataAccess;
using Goblin.Domain;
using Microsoft.EntityFrameworkCore;

namespace Goblin.BackgroundJobs.Jobs;

public class SendToUsersTasks(BotDbContext db, IEnumerable<ISender> senders)
{
    public async Task SendToAll(string text, IReadOnlyCollection<string> attachments, bool isSendKeyboard, ConsumerType type)
    {
        var keyboard = isSendKeyboard ? DefaultKeyboards.GetDefaultKeyboard() : null;
        if(type == ConsumerType.AllInOne)
        {
            var groupedByConsumerType = db.BotUsers.AsEnumerable().GroupBy(x => x.ConsumerType);
            foreach(var consumersGroup in groupedByConsumerType)
            {
                var users = consumersGroup.Select(x => x.Id).ToList();
                var sender = senders.First(x => x.ConsumerType == consumersGroup.Key);
                await sender.SendToMany(users, text, keyboard, attachments);
            }
        }
        else
        {
            var users = await db.BotUsers
                                 .Where(x => x.ConsumerType == type)
                                 .Select(x => x.Id)
                                 .ToListAsync();
            var sender = senders.First(x => x.ConsumerType == type);
            await sender.SendToMany(users, text, keyboard, attachments);
        }
    }

    public async Task SendToId(long chatId, string text, IReadOnlyCollection<string> attachments, ConsumerType type)
    {
        var sender = senders.First(x => x.ConsumerType == type);
        await sender.Send(chatId, text, attachments: attachments);
    }
}