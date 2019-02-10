using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Goblin.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Vk;
using Vk.Models;
using Vk.Models.Messages;
using User = Goblin.Models.User;

namespace Goblin.Bot
{
    public class Handler
    {
        private readonly MainContext _db;
        private readonly IConfiguration _config;
        private readonly CommandExecutor _executor;
        private readonly VkApi _api;

        public Handler(MainContext db, IConfiguration config, CommandExecutor exec, VkApi api)
        {
            _db = db;
            _config = config;
            _executor = exec;
            _api = api;
        }

        public async Task<string> Handle(CallbackResponse callbackResponse)
        {
            var type = callbackResponse.Type;
            var dict = new Dictionary<string, Func<CallbackResponse, Task<string>>>
            {
                ["confirmation"] = Confirmation,
                ["message_new"] = MessageNew,
                ["message_deny"] = MessageDeny,
                ["group_join"] = GroupJoin,
                ["group_leave"] = GroupLeave
            };

            return await dict[type](callbackResponse);
        }

        private async Task<string> Confirmation(CallbackResponse r)
        {
            return _config["Config:ConfirmationCode"];
        }

        private async Task<string> MessageNew(CallbackResponse obj)
        {
            var message = Message.FromJson(obj.Object.ToString());
            if(message.FromId != message.PeerId)
            {
                //TODO: add conv to db
                var match = Regex.Match(message.Text, @"\[club146048760\|.*\] (.*)").Groups[1].Value;
                if(!string.IsNullOrEmpty(match))
                {
                    message.Text = match;
                }

                //TODO: оповещение о том, что гоблину не нужен доступ ко всей переписке?
            }

            if(_db.GetUsers().All(x => x.Vk != message.FromId))
            {
                await _db.Users.AddAsync(new User { Vk = message.FromId });
                await _db.SaveChangesAsync();
            }

            var response = await _executor.ExecuteCommand(message);
            await _api.Messages.Send(message.PeerId, response.Text, response.Attachments, response.Keyboard);
            return "ok";
        }

        private async Task<string> MessageDeny(CallbackResponse obj)
        {
            var deny = Vk.Models.Responses.MessageDeny.FromJson(obj.Object.ToString());

            var userName = await _api.Users.Get(deny.UserId);
            await _api.Messages.Send(_db.GetAdmins(), $"@id{deny.UserId} ({userName}) запретил сообщения");

            return "ok";
        }

        private async Task<string> GroupJoin(CallbackResponse obj)
        {
            var join = Vk.Models.Responses.GroupJoin.FromJson(obj.Object.ToString());
            var userName = await _api.Users.Get(join.UserId);

            await _api.Messages.Send(_db.GetAdmins(), $"@id{join.UserId} ({userName}) подписался");
            return "ok";
        }

        private async Task<string> GroupLeave(CallbackResponse obj)
        {
            var leave = Vk.Models.Responses.GroupLeave.FromJson(obj.Object.ToString());
            var userId = leave.UserId;
            if(await _db.Users.AnyAsync(x => x.Vk == userId))
            {
                _db.Users.Remove(_db.Users.First(x => x.Vk == userId));
                await _db.SaveChangesAsync();
            }

            var userName = await _api.Users.Get(userId);
            await _api.Messages.Send(_db.GetAdmins(), $"@id{userId} ({userName}) отписался");
            return "ok";
        }
    }
}
