using Goblin.Bot;
using Goblin.Helpers;
using Goblin.Models;
using Goblin.Models.Keyboard;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Goblin.Controllers
{
    public class ApiController : Controller
    {
        public async Task<string> Handler([FromBody] dynamic body)
        {
            var eventType = body["type"].ToString();
            int userID;
            string userName;
            switch (eventType)
            {
                case "confirmation":
                    return VkHelper.ConfirmationToken;

                case "message_new":
                    userID = int.Parse(body["object"]["from_id"].ToString());
                    int convID = int.Parse(body["object"]["peer_id"].ToString());
                    var msg = body["object"]["text"].ToString();
                    if (userID != convID)
                    {
                        var b = Regex.Match(msg, @"\[club146048760\|.*\] (.*)").Groups[1].Value;
                        if (b != "")
                            msg = b;
                    }

                    if (!DbHelper.Db.Users.Any(x => x.Vk == userID))
                    {
                        await DbHelper.Db.Users.AddAsync(new User { Vk = userID });
                        await DbHelper.Db.SaveChangesAsync();
                    }

                    (string Message, Keyboard Keyboard) forSend = await CommandsList.ExecuteCommand(msg, userID);
                    await VkHelper.SendMessage(convID, forSend.Message, kb: forSend.Keyboard);
                    break;

                case "group_join":
                    userID = int.Parse(body["object"]["user_id"].ToString());
                    userName = await VkHelper.GetUserName(userID);
                    await VkHelper.SendMessage(VkHelper.DevelopersID, $"@id{userID} ({userName}) подписался!");
                    break;

                case "group_leave":
                case "message_deny":
                    userID = int.Parse(body["object"]["user_id"].ToString());
                    if (await DbHelper.Db.Users.AnyAsync(x => x.Vk == userID))
                    {
                        DbHelper.Db.Users.Remove(DbHelper.Db.Users.First(x => x.Vk == userID));
                        await DbHelper.Db.SaveChangesAsync();
                    }

                    userName = await VkHelper.GetUserName(userID);
                    await VkHelper.SendMessage(VkHelper.DevelopersID, $"@id{userID} ({userName}) отписался!");
                    break;
            }

            return "ok";
        }

        public async Task SendMessage(string msg)
        {
            if (!ModelState.IsValid) return;
            await VkHelper.SendMessage(DbHelper.GetUsers().Select(x => x.Vk).ToList(), msg);
        }

        public async Task SendWeather()
        {
            await Task.Factory.StartNew(async () =>
            {
                var grouped = DbHelper.GetWeatherUsers().GroupBy(x => x.City);
                foreach (var group in grouped)
                {
                    var ids = group.Select(x => x.Vk).ToList();
                    await VkHelper.SendMessage(ids, await WeatherHelper.GetWeather(group.Key));
                    await Task.Delay(1500);
                }
            });
        }

        public async Task SendSchedule()
        {
            if (DateTime.Now.DayOfWeek == DayOfWeek.Sunday) return;
            await Task.Factory.StartNew(async () =>
            {
                var grouped = DbHelper.GetScheduleUsers().GroupBy(x => x.Group);
                foreach (var group in grouped)
                {
                    var ids = group.Select(x => x.Vk).ToList();
                    var schedule = await ScheduleHelper.GetScheduleAtDate(DateTime.Today, group.Key);
                    await VkHelper.SendMessage(ids, schedule);
                    await Task.Delay(1500);
                }
            });
        }

        public async Task SendToPesi() //TODO: сделать параметры и удалить дублирующуюся штуку внизу
        {
            var konfa = 2000000003;
            await VkHelper.SendToConversation(konfa, 351617, "Архангельск");
        }

        public async Task SendToIgor()
        {
            var konfa = 2000000005;
            await VkHelper.SendToConversation(konfa, 351616);
        }
    }
}