using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Goblin.Application.Core.Abstractions;
using Goblin.Application.Core.Models;
using Goblin.Application.Core.Results.Failed;
using Goblin.Application.Core.Results.Success;
using Goblin.DataAccess;
using Goblin.Domain;
using Goblin.Domain.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Goblin.Application.Core.Commands.Text
{
    public class SendToAdminCommand : ITextCommand
    {
        public bool IsAdminCommand => false;
        public string[] Aliases => new[] { "админ", "сообщение" };

        private readonly BotDbContext _db;
        private readonly IEnumerable<ISender> _senders;

        public SendToAdminCommand(BotDbContext db, IEnumerable<ISender> senders)
        {
            _db = db;
            _senders = senders;
        }

        public async Task<IResult> Execute(Message msg, BotUser user)
        {
            var text = string.Join(' ', msg.CommandParameters);
            if(string.IsNullOrWhiteSpace(text))
            {
                return new FailedResult("Введите текст сообщения.");
            }

            var message = string.Empty;
            var adminUsers = new List<long>();
            ISender sender = null;
            if(user.ConsumerType == ConsumerType.Vkontakte)
            {
                message = $"Сообщение от {msg.UserTag}:\n{text}";
                adminUsers = await _db.VkBotUsers.Where(x => x.IsAdmin).Select(x => x.Id).ToListAsync();
                sender = _senders.FirstOrDefault(x => x.ConsumerType == ConsumerType.Vkontakte);
            }
            else if(user.ConsumerType == ConsumerType.Telegram)
            {
                message = $"Сообщение от {msg.UserTag}:\n{text}";
                adminUsers = await _db.TgBotUsers.Where(x => x.IsAdmin).Select(x => x.Id).ToListAsync();
                sender = _senders.FirstOrDefault(x => x.ConsumerType == ConsumerType.Telegram);
            }

            foreach(var admin in adminUsers)
            {
                await sender.Send(admin, message);
            }

            return new SuccessfulResult("Ваше сообщение успешно отправлено администрации");
        }
    }
}