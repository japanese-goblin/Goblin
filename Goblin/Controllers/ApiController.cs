using System;
using System.Linq;
using System.Text.RegularExpressions;
using Goblin.Bot;
using Goblin.Helpers;
using Goblin.Models;
using Microsoft.AspNetCore.Mvc;

namespace Goblin.Controllers
{
    public class ApiController : Controller
    {
        private readonly MainContext db;

        public ApiController(MainContext context)
        {
            db = context;
        }

        public string Handler([FromBody] dynamic body)
        {
            var eventType = body["type"].ToString();
            int userID;
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
                        db.Users.Add(new User {Vk = userID});
                        db.SaveChanges();
                    }

                    VkHelper.SendMessage(convID, CommandsList.ExecuteCommand(msg, userID));
                    break;

                case "group_join":
                    userID = int.Parse(body["object"]["user_id"].ToString());
                    //db.Users.Add(new User() { Vk = userID });
                    //db.SaveChanges();
                    //{"type":"group_join","object":{"user_id":***REMOVED***,"join_type":"join"},"group_id":146286422}
                    VkHelper.SendMessage(VkHelper.DevelopersID, $"@id{userID} ({VkHelper.GetUserName(userID)}) подписался!");
                    break;

                case "group_leave":
                case "message_deny":
                    userID = int.Parse(body["object"]["user_id"].ToString());
                    //{"type":"group_leave","object":{"user_id":***REMOVED***,"self":1},"group_id":146286422}
                    //{"type":"message_deny","object":{"user_id":***REMOVED***},"group_id":146286422}
                    if (db.Users.Any(x => x.Vk == userID))
                    {
                        db.Users.Remove(db.Users.First(x => x.Vk == userID));
                        db.SaveChanges();
                    }

                    VkHelper.SendMessage(VkHelper.DevelopersID, $"@id{userID} ({VkHelper.GetUserName(userID)}) отписался!");
                    break;
            }

            return "ok";
        }

        public bool SendMessage(string msg)
        {
            return VkHelper.SendMessage(db.Users.Select(x => x.Vk).ToList(), msg);
        }

        public void SendWeather()
        {
            var grouped = db.Users.Where(x => x.City != "" && x.Weather).GroupBy(x => x.City);
            foreach (var group in grouped)
            {
                var ids = group.Select(x => x.Vk).ToList();
                VkHelper.SendMessage(ids, WeatherHelper.GetWeather(group.Key));
            }
        }

        public void SendSchedule()
        {
            var day = DateTime.Now.DayOfWeek;
            if (day == DayOfWeek.Saturday) return;
            var grouped = db.Users.Where(x => x.Group != 0 && x.Schedule).GroupBy(x => x.Group);
            foreach (var group in grouped)
            {
                var ids = group.Select(x => x.Vk).ToList();
                var schedule = ScheduleHelper.GetScheduleAtDate(DateTime.Today, group.Key);
                VkHelper.SendMessage(ids, schedule);
            }
        }
    }
}