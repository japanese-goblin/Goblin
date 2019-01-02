using Goblin.Bot;
using Goblin.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Goblin.Schedule;
using Goblin.Vk;
using Goblin.Vk.Models;
using Goblin.Weather;
using User = Goblin.Models.User;

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
                    return VkMethods.ConfirmationToken;

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
                    await VkMethods.SendMessage(convID, forSend.Message, kb: forSend.Keyboard);
                    break;

                case "group_join":
                    userID = int.Parse(body["object"]["user_id"].ToString());
                    userName = await VkMethods.GetUserName(userID);
                    await VkMethods.SendMessage(VkMethods.DevelopersID, $"@id{userID} ({userName}) подписался!");
                    break;

                case "group_leave":
                case "message_deny":
                    userID = int.Parse(body["object"]["user_id"].ToString());
                    if (await DbHelper.Db.Users.AnyAsync(x => x.Vk == userID))
                    {
                        DbHelper.Db.Users.Remove(DbHelper.Db.Users.First(x => x.Vk == userID));
                        await DbHelper.Db.SaveChangesAsync();
                    }

                    userName = await VkMethods.GetUserName(userID);
                    await VkMethods.SendMessage(VkMethods.DevelopersID, $"@id{userID} ({userName}) отписался!");
                    break;
            }

            return "ok";
        }

        public async Task SendMessage(string msg)
        {
            if (!ModelState.IsValid) return;
            await VkMethods.SendMessage(DbHelper.GetUsers().Select(x => x.Vk).ToArray(), msg);
        }

        public async Task SendWeather()
        {
            await Task.Factory.StartNew(async () =>
            {
                var grouped = DbHelper.GetWeatherUsers().GroupBy(x => x.City);
                foreach (var group in grouped)
                {
                    var ids = group.Select(x => x.Vk).ToArray();
                    await VkMethods.SendMessage(ids, await WeatherInfo.GetWeather(group.Key));
                    await Task.Delay(700); //TODO - 3 запроса в секунду
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
                    var ids = group.Select(x => x.Vk).ToArray();
                    var schedule = await StudentsSchedule.GetScheduleAtDate(DateTime.Today, group.Key);
                    await VkMethods.SendMessage(ids, schedule);
                    await Task.Delay(500); //TODO - 3 запроса в секунду
                }
            });
        }

        //TODO 
        //public async Task SendToConv(int id, int group = 0, string city = "")
        //{
        //    if (!ModelState.IsValid && ScheduleHelper.IsCorrectGroup(group)) return;

        //    id = 2000000000 + id;

        //    if (!string.IsNullOrEmpty(city) && await WeatherHelper.CheckCity(city))
        //    {
        //        await VkMethods.SendToConversation(id, group, city);
        //    }
        //    else
        //    {
        //        await VkMethods.SendToConversation(id, group);
        //    }
        //}
    }
}