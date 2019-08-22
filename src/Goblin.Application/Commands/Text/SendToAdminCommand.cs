using System.Linq;
using System.Threading.Tasks;
using Goblin.Application.Abstractions;
using Goblin.Application.Extensions;
using Goblin.Application.Results.Failed;
using Goblin.Application.Results.Success;
using Goblin.DataAccess;
using Goblin.Domain.Entities;
using VkNet.Abstractions;
using VkNet.Model;
using VkNet.Model.RequestParams;

namespace Goblin.Application.Commands.Text
{
    public class SendToAdminCommand : ITextCommand
    {
        public bool IsAdminCommand => false;
        public string[] Aliases => new[] { "админ", "сообщение" };

        private readonly IVkApi _api;
        private readonly BotDbContext _db;

        public SendToAdminCommand(IVkApi api, BotDbContext db)
        {
            _api = api;
            _db = db;
        }

        public async Task<IResult> Execute(Message msg, BotUser user)
        {
            var param = msg.GetCommandParameters();
            if(param.Length < 2)
            {
                return new FailedResult("Введите текст сообщения.");
            }

            var msgText = $"Сообщение от @id{msg.FromId}:\n{string.Join(' ', param)}";

            await _api.Messages.SendToUserIdsWithRandomId(new MessagesSendParams
            {
                UserIds = _db.BotUsers.Where(x => x.IsAdmin).Select(x => x.VkId),
                Message = msgText
            });
            return new SuccessfulResult
            {
                Message = "Ваше сообщение успешно отправлено."
            };
        }
    }
}