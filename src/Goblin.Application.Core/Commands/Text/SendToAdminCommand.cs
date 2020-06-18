// using System.Linq;
// using System.Threading.Tasks;
// using Goblin.Application.Core.Abstractions;
// using Goblin.Application.Core.Extensions;
// using Goblin.Application.Core.Results.Failed;
// using Goblin.Application.Core.Results.Success;
// using Goblin.DataAccess;
// using Goblin.Domain.Entities;
// using VkNet.Abstractions;
// using VkNet.Model;
// using VkNet.Model.RequestParams;
//
//TODO:
// namespace Goblin.Application.Core.Commands.Text
// {
//     public class SendToAdminCommand : ITextCommand
//     {
//         public bool IsAdminCommand => false;
//         public string[] Aliases => new[] { "админ", "сообщение" };
//
//         private readonly IVkApi _api;
//         private readonly BotDbContext _db;
//
//         public SendToAdminCommand(IVkApi api, BotDbContext db)
//         {
//             _api = api;
//             _db = db;
//         }
//
//         public async Task<IResult> Execute(Message msg, BotUser user)
//         {
//             var param = string.Join(' ', msg.GetCommandParameters());
//             if(string.IsNullOrWhiteSpace(param))
//             {
//                 return new FailedResult("Введите текст сообщения.");
//             }
//
//             var msgText = $"Сообщение от @id{msg.FromId}:\n{string.Join(' ', param)}";
//
//             await _api.Messages.SendToUserIdsWithRandomId(new MessagesSendParams
//             {
//                 UserIds = _db.BotUsers.Where(x => x.IsAdmin).Select(x => x.VkId),
//                 Message = msgText
//             });
//             return new SuccessfulResult
//             {
//                 Message = "Ваше сообщение успешно отправлено."
//             };
//         }
//     }
// }