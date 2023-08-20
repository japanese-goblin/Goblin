using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Goblin.Application.Core;
using Goblin.DataAccess;
using Goblin.Domain;

namespace Goblin.BackgroundJobs.Jobs;

public class SendToUsersTasks
{
    private readonly BotDbContext _db;
    private readonly IEnumerable<ISender> _senders;

    public SendToUsersTasks(BotDbContext db, IEnumerable<ISender> senders)
    {
        _db = db;
        _senders = senders;
    }

    public async Task SendToAll(string text, ICollection<string> attachments, bool isSendKeyboard, ConsumerType type)
    {
        var keyboard = isSendKeyboard ? DefaultKeyboards.GetDefaultKeyboard() : null;
        if(type == ConsumerType.AllInOne)
        {
            var groupedByConsumerType = _db.BotUsers.AsEnumerable().GroupBy(x => x.ConsumerType);
            foreach(var consumersGroup in groupedByConsumerType)
            {
                var users = consumersGroup.Select(x => x.Id).AsEnumerable();
                var sender = _senders.First(x => x.ConsumerType == consumersGroup.Key);
                await sender.SendToMany(users, text, keyboard, attachments);
            }
        }
        else
        {
            var users = _db.BotUsers.Where(x => x.ConsumerType == type).Select(x => x.Id).AsEnumerable();
            var sender = _senders.First(x => x.ConsumerType == type);
            await sender.SendToMany(users, text, keyboard, attachments);
        }
    }
    
    public async Task SendToId(long chatId, string text, ICollection<string> attachments, ConsumerType type)
    {
        var sender = _senders.First(x => x.ConsumerType == type);
        await sender.Send(chatId, text, attachments: attachments);
    }
}