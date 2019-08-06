using System;
using System.Threading.Tasks;
using Goblin.DataAccess;
using Goblin.Domain.Entities;
using VkNet.Enums.SafetyEnums;
using VkNet.Model;
using VkNet.Model.GroupUpdate;
using VkNet.Utils;

namespace Goblin.Application
{
    public class CallbackHandler
    {
        private readonly CommandsService _service;
        private readonly BotDbContext _db;

        public const string DefaultResult = "ok";

        public CallbackHandler(CommandsService service, BotDbContext db)
        {
            _service = service;
            _db = db;
        }

        public async Task<string> Handle(VkResponse response)
        {
            var upd = GroupUpdate.FromJson(response);
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
                throw new ArgumentOutOfRangeException(nameof(upd.Type), "Отсутствует обработчик события");
            }

            return DefaultResult;
        }

        private async Task MessageNew(Message msg)
        {
            var user = _db.BotUsers.Find(msg.FromId);
            if(user is null)
            {
                user = _db.BotUsers.Add(new BotUser(msg.FromId.Value)).Entity;
                await _db.SaveChangesAsync();
            }
            
            await _service.ExecuteCommand(msg, user);
        }

        public async Task GroupLeave(GroupLeave leave)
        {
            //TODO: 
        }
        
        public async Task GroupJoin(GroupJoin join)
        {
            //TODO: 
        }
    }
}