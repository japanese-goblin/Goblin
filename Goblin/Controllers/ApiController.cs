using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Goblin.Bot;
using Goblin.Helpers;
using Goblin.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Goblin.Controllers
{
    public class ApiController : Controller
    {
        private readonly MainContext db;

        public ApiController()
        {
            db = new MainContext();
        }

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

                    if (!db.Users.Any(x => x.Vk == userID))
                    {
                        await db.Users.AddAsync(new User {Vk = userID});
                        await db.SaveChangesAsync();
                    }

                    await VkHelper.SendMessage(convID, await CommandsList.ExecuteCommand(msg, userID));
                    break;

                case "group_join":
                    userID = int.Parse(body["object"]["user_id"].ToString());
                    userName = await VkHelper.GetUserName(userID);
                    await VkHelper.SendMessage(VkHelper.DevelopersID, $"@id{userID} ({userName}) подписался!");
                    break;

                case "group_leave":
                case "message_deny":
                    userID = int.Parse(body["object"]["user_id"].ToString());
                    if (await db.Users.AnyAsync(x => x.Vk == userID))
                    {
                        db.Users.Remove(db.Users.First(x => x.Vk == userID));
                        await db.SaveChangesAsync();
                    }

                    userName = await VkHelper.GetUserName(userID);
                    await VkHelper.SendMessage(VkHelper.DevelopersID, $"@id{userID} ({userName}) отписался!");
                    break;
            }

            return "ok";
        }

        public async Task<bool> SendMessage(string msg)
        {
            return await VkHelper.SendMessage(db.Users.Select(x => x.Vk).ToList(), msg);
        }

        public async void SendWeather()
        {
            var grouped = db.Users.Where(x => x.City != "" && x.Weather).GroupBy(x => x.City);
            foreach (var group in grouped)
            {
                var ids = group.Select(x => x.Vk).ToList();
                await VkHelper.SendMessage(ids, await WeatherHelper.GetWeather(group.Key));
            }
        }

        public async void SendSchedule()
        {
            var grouped = db.Users.Where(x => x.Group != 0 && x.Schedule).GroupBy(x => x.Group);
            foreach (var group in grouped)
            {
                var ids = group.Select(x => x.Vk).ToList();
                var schedule = await ScheduleHelper.GetScheduleAtDate(DateTime.Today, group.Key);
                await VkHelper.SendMessage(ids, schedule);
            }
        }

        public async Task SendToPesi()
        {
            var konfa = 2000000003;
            var schedule = await ScheduleHelper.GetScheduleAtDate(DateTime.Now, 351617);
            var weather = await WeatherHelper.GetWeather("Архангельск");

            await VkHelper.SendMessage(konfa, schedule);
            await VkHelper.SendMessage(konfa, weather);
        }
    }
}