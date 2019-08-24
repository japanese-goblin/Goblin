using System;
using System.Linq;
using System.Threading.Tasks;
using Goblin.Application.Extensions;
using Goblin.Application.Results.Failed;
using Goblin.Application.Results.Success;
using Goblin.DataAccess;
using Goblin.Domain.Entities;
using Serilog;
using VkNet.Abstractions;
using VkNet.Enums.SafetyEnums;
using VkNet.Model;
using VkNet.Model.GroupUpdate;
using VkNet.Model.RequestParams;
using VkNet.Utils;

namespace Goblin.Application
{
    public class CallbackHandler
    {
        public const string DefaultResult = "ok";
        private readonly BotDbContext _db;
        private readonly CommandsService _service;
        private readonly IVkApi _vkApi;

        public CallbackHandler(CommandsService service, BotDbContext db, IVkApi vkApi)
        {
            _service = service;
            _db = db;
            _vkApi = vkApi;
        }

        public async Task<string> Handle(VkResponse response)
        {
            var upd = GroupUpdate.FromJson(response);
            Log.Information("Обработка события с типом {0}", upd.Type);
            if(upd.Type == GroupUpdateType.MessageNew)
            {
                await MessageNew(upd.Message);
            }
            else if(upd.Type == GroupUpdateType.GroupLeave)
            {
                await GroupLeave(upd.GroupLeave);
            }
            else if(upd.Type == GroupUpdateType.GroupJoin)
            {
                await GroupJoin(upd.GroupJoin);
            }
            else
            {
                Log.Fatal("Обработчик для события {0} не найден", upd.Type);
                throw new ArgumentOutOfRangeException(nameof(upd.Type), "Отсутствует обработчик события");
            }

            return DefaultResult;
        }

        private async Task MessageNew(Message msg)
        {
            var user = _db.BotUsers.Find(msg.FromId);
            if(user is null)
            {
                Log.Information("Пользователь с id {0} не найден. Создание записи.", msg.FromId);
                user = _db.BotUsers.Add(new BotUser(msg.FromId.Value)).Entity;
                _db.Subscribes.Add(new Subscribe(msg.FromId.Value, false, false));
                await _db.SaveChangesAsync();
                Log.Information("Пользователь создан");
            }

            Log.Information("Обработка сообщения");
            var result = await _service.ExecuteCommand(msg, user);
            Log.Information("Сообщение обработано");

            if(result is FailedResult failed)
            {
                if(result is CommandNotFoundResult && !user.IsErrorsEnabled)
                {
                    // если команда не найдена, и у юзера отключены ошибки
                    return;
                }

                await _vkApi.Messages.SendError(failed.ToString(), msg.PeerId.Value);
            }
            else
            {
                var success = result as SuccessfulResult;
                await _vkApi.Messages.SendWithRandomId(new MessagesSendParams
                {
                    Message = success.Message,
                    Attachments = success.Attachments,
                    Keyboard = success.Keyboard,
                    PeerId = msg.PeerId.Value
                });
            }
        }

        public async Task GroupLeave(GroupLeave leave)
        {
            Log.Information("Пользователь id{0} покинул группу", leave.UserId);
            var admins = _db.BotUsers.Where(x => x.IsAdmin).Select(x => x.VkId);
            var vkUser = (await _vkApi.Users.GetAsync(new[] { leave.UserId.Value })).First();
            var userName = $"{vkUser.FirstName} {vkUser.LastName}";
            await _vkApi.Messages.SendToUserIdsWithRandomId(new MessagesSendParams
            {
                Message = $"@id{leave.UserId} ({userName}) отписался :С",
                UserIds = admins
            });
        }

        public async Task GroupJoin(GroupJoin join)
        {
            Log.Information("Пользователь id{0} вступил в группу", join.UserId);
            var admins = _db.BotUsers.Where(x => x.IsAdmin).Select(x => x.VkId);
            var vkUser = (await _vkApi.Users.GetAsync(new[] { join.UserId.Value })).First();
            var userName = $"{vkUser.FirstName} {vkUser.LastName}";
            await _vkApi.Messages.SendToUserIdsWithRandomId(new MessagesSendParams
            {
                Message = $"@id{join.UserId} ({userName}) подписался!",
                UserIds = admins
            });
        }
    }
}