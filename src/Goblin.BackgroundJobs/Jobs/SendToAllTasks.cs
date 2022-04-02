using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Goblin.Application.Core;
using Goblin.DataAccess;
using Goblin.Domain;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace Goblin.BackgroundJobs.Jobs;

public class SendToUsersTasks
{
    private readonly BotDbContext _db;
    private readonly IEnumerable<ISender> _senders;

    //TODO: ISender
    public SendToUsersTasks(BotDbContext db, IEnumerable<ISender> senders)
    {
        _db = db;
        _senders = senders;
        Log.ForContext<SendToUsersTasks>();
    }

    public async Task SendToAll(string text, string[] attachments, bool isSendKeyboard, ConsumerType type)
    {
        var keyboard = isSendKeyboard ? DefaultKeyboards.GetDefaultKeyboard() : null;
        if(type == ConsumerType.AllInOne)
        {
            //TODO: users in one table, group by ConsumerType
            var vkSender = _senders.FirstOrDefault(x => x.ConsumerType == type);
            var vkUserIds = _db.VkBotUsers
                               .AsNoTracking()
                               .Select(x => x.Id)
                               .AsEnumerable();
            await vkSender.SendToMany(vkUserIds, text, keyboard, attachments);

            var tgSender = _senders.FirstOrDefault(x => x.ConsumerType == type);
            var tgUserIds = _db.VkBotUsers
                               .AsNoTracking()
                               .Select(x => x.Id)
                               .AsEnumerable();
            await tgSender.SendToMany(tgUserIds, text, keyboard, attachments);
        }

        if(type == ConsumerType.Vkontakte)
        {
            var sender = _senders.FirstOrDefault(x => x.ConsumerType == type);
            var userIds = _db.VkBotUsers
                             .AsNoTracking()
                             .Select(x => x.Id)
                             .AsEnumerable();
            await sender.SendToMany(userIds, text, keyboard, attachments);
            return;
        }

        if(type == ConsumerType.Telegram)
        {
            var sender = _senders.FirstOrDefault(x => x.ConsumerType == type);
            var userIds = _db.TgBotUsers
                             .AsNoTracking()
                             .Select(x => x.Id)
                             .AsEnumerable();
            await sender.SendToMany(userIds, text, keyboard, attachments);
        }
    }

    public async Task SendToId(long chatId, string text, string[] attachments, ConsumerType type)
    {
        var sender = _senders.FirstOrDefault(x => x.ConsumerType == type);
        await sender.Send(chatId, text, attachments: attachments);
    }
}