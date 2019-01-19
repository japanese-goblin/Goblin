using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Goblin.Helpers;
using Microsoft.EntityFrameworkCore;
using Vk;
using Vk.Models;
using Vk.Models.Messages;
using User = Goblin.Models.User;

namespace Goblin.Bot
{
    public static class Handler
    {
        public static async Task<string> Handle(CallbackResponse callbackResponse)
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

        private static async Task<string> Confirmation(CallbackResponse r)
        {
            return Settings.ConfirmationToken;
        }

        private static async Task<string> MessageNew(CallbackResponse obj)
        {
            var message = Message.FromJson(obj.Object.ToString());
            if (message.FromId != message.PeerId)
            {
                //TODO: add conv to db
                var match = Regex.Match(message.Text, @"\[club146048760\|.*\] (.*)").Groups[1].Value;
                if (!string.IsNullOrEmpty(match))
                {
                    message.Text = match;
                }

                //TODO: оповещение о том, что гоблину не нужен доступ ко всей переписке?
            }

            if (!DbHelper.GetUsers().Any(x => x.Vk == message.FromId))
            {
                await DbHelper.Db.Users.AddAsync(new User {Vk = message.FromId});
                await DbHelper.Db.SaveChangesAsync();
            }

            var response = await CommandsList.ExecuteCommand(message);
            await VkApi.Messages.Send(message.PeerId, response.Text, response.Attachments, response.Keyboard);
            return "ok";
        }

        private static async Task<string> MessageDeny(CallbackResponse obj)
        {
            var deny = Vk.Models.Responses.MessageDeny.FromJson(obj.Object.ToString());
            var userID = deny.UserId;

            var userName = await VkApi.Users.GetUserName(userID);
            await VkApi.Messages.Send(DbHelper.GetAdmins(), $"@id{userID} ({userName}) запретил сообщения");

            return "ok";
        }

        private static async Task<string> GroupJoin(CallbackResponse obj)
        {
            var join = Vk.Models.Responses.GroupJoin.FromJson(obj.Object.ToString());
            var userID = join.UserId;
            var userName = await VkApi.Users.GetUserName(userID);

            await VkApi.Messages.Send(DbHelper.GetAdmins(), $"@id{userID} ({userName}) подписался");
            return "ok";
        }

        private static async Task<string> GroupLeave(CallbackResponse obj)
        {
            var leave = Vk.Models.Responses.GroupLeave.FromJson(obj.Object.ToString());
            var userID = leave.UserId;
            if (await DbHelper.Db.Users.AnyAsync(x => x.Vk == userID))
            {
                DbHelper.Db.Users.Remove(DbHelper.Db.Users.First(x => x.Vk == userID));
                await DbHelper.Db.SaveChangesAsync();
            }

            var userName = await VkApi.Users.GetUserName(userID);
            await VkApi.Messages.Send(DbHelper.GetAdmins(), $"@id{userID} ({userName}) отписался");
            return "ok";
        }
    }
}