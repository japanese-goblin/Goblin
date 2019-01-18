using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Goblin.Helpers;
using Microsoft.EntityFrameworkCore;
using Vk;
using Vk.Models;
using User = Goblin.Models.User;

namespace Goblin.Bot
{
    public static class Handler
    {
        public static async Task<string> Handle(Response response)
        {
            var type = response.Type;
            var dict = new Dictionary<string, Func<Response, Task<string>>>
            {
                ["confirmation"] = Confirmation,
                ["message_new"] = MessageNew,
                ["message_deny"] = MessageDeny,
                ["group_join"] = GroupJoin,
                ["group_leave"] = GroupLeave
            };

            return await dict[type](response);
        }

        private static async Task<string> Confirmation(Response r)
        {
            return Settings.ConfirmationToken;
        }

        private static async Task<string> MessageNew(Response obj)
        {
            var message = Vk.Models.Messages.Message.FromJson(obj.Object.ToString());
            var userID = message.FromId;
            var convId = message.PeerId;
            if (userID != convId)
            {
                //TODO: add conv to db
                var match = Regex.Match(message.Text, @"\[club146048760\|.*\] (.*)").Groups[1].Value;
                if (string.IsNullOrEmpty(match))
                {
                    message.Text = match;
                }

                //TODO: оповещение о том, что гоблину не нужен доступ ко всей переписке?
            }

            if (!DbHelper.GetUsers().Any(x => x.Vk == userID))
            {
                await DbHelper.Db.Users.AddAsync(new User {Vk = userID});
                await DbHelper.Db.SaveChangesAsync();
            }

            var (msg, keyboard) = await CommandsList.ExecuteCommand(message.Text, userID);
            await VkApi.Messages.Send(convId, msg, kb: keyboard);
            return "ok";
        }

        private static async Task<string> MessageDeny(Response obj)
        {
            var deny = Vk.Models.Responses.MessageDeny.FromJson(obj.Object.ToString());
            var userID = deny.UserId;

            var userName = await VkApi.Users.GetUserName(userID);
            await VkApi.Messages.Send(DbHelper.GetAdmins(), $"@id{userID} ({userName}) запретил сообщения");

            return "ok";
        }

        private static async Task<string> GroupJoin(Response obj)
        {
            var join = Vk.Models.Responses.GroupJoin.FromJson(obj.Object.ToString());
            var userID = join.UserId;
            var userName = await VkApi.Users.GetUserName(userID);

            await VkApi.Messages.Send(DbHelper.GetAdmins(), $"@id{userID} ({userName}) подписался");
            return "ok";
        }

        private static async Task<string> GroupLeave(Response obj)
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