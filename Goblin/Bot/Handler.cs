using Goblin.Helpers;
using Goblin.Models;
using Goblin.Vk;
using Goblin.Vk.Models;
using Goblin.Vk.Models.Responses;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

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
            return VkMethods.ConfirmationToken;
        }

        private static async Task<string> MessageNew(Response obj)
        {
            var msg = Vk.Models.Responses.MessageNew.FromJson(obj.Object.ToString()) as MessageNew;
            var userID = msg.FromId;
            var convId = msg.PeerId;
            if (userID != convId)
            {
                //TODO: add conv to db
                var b = Regex.Match(msg.Text, @"\[club146048760\|.*\] (.*)").Groups[1].Value;
                if (b != "")
                    msg.Text = b;
                //TODO: оповещение о том, что гоблину не нужен доступ ко всей переписке?
            }

            if (!DbHelper.GetUsers().Any(x => x.Vk == userID))
            {
                await DbHelper.Db.Users.AddAsync(new User { Vk = userID });
                await DbHelper.Db.SaveChangesAsync();
            }

            var forSend = await CommandsList.ExecuteCommand(msg.Text, userID);
            await VkMethods.SendMessage(convId, forSend.Message, kb: forSend.Keyboard);
            return "ok";
        }

        private static async Task<string> MessageDeny(Response obj)
        {
            var deny = Vk.Models.Responses.MessageDeny.FromJson(obj.Object.ToString()) as MessageDeny;
            var userID = deny.UserId;

            var userName = await VkMethods.GetUserName(userID);
            await VkMethods.SendMessage(VkMethods.DevelopersID, $"@id{userID} ({userName}) запретил сообщения");

            return "ok";
        }

        private static async Task<string> GroupJoin(Response obj)
        {
            var join = Vk.Models.Responses.GroupJoin.FromJson(obj.Object.ToString()) as GroupJoin;
            var userID = join.UserId;
            var userName = await VkMethods.GetUserName(userID);

            await VkMethods.SendMessage(VkMethods.DevelopersID, $"@id{userID} ({userName}) подписался");
            return "ok";
        }

        private static async Task<string> GroupLeave(Response obj)
        {
            var leave = Vk.Models.Responses.GroupLeave.FromJson(obj.Object.ToString()) as GroupLeave;
            var userID = leave.UserId;
            if (await DbHelper.Db.Users.AnyAsync(x => x.Vk == userID))
            {
                DbHelper.Db.Users.Remove(DbHelper.Db.Users.First(x => x.Vk == userID));
                await DbHelper.Db.SaveChangesAsync();
            }

            var userName = await VkMethods.GetUserName(userID);
            await VkMethods.SendMessage(VkMethods.DevelopersID, $"@id{userID} ({userName}) отписался");
            return "ok";
        }
    }
}