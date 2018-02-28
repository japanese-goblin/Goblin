using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Goblin.Models;
using Ical.Net;
using Microsoft.AspNetCore.Mvc;

namespace Goblin.Controllers
{
    public class CronController : Controller
    {
        private MainContext db;

        public CronController(MainContext context)
        {
            db = context;
        }

        public void SendWeather()
        {
            var grouped = db.Users.Where(x => x.CityNumber != 0 && x.Weather).GroupBy(x => x.City);
            foreach (var group in grouped)
            {
                var ids = group.Select(x => x.Vk).ToList();
                Utils.SendMessage(ids, $"В городе {group.Key} очень хорошая погода!"); //TODO: дополнить
            }
        }

        public void SendRemind()
        {
            //TODO: ?????
            var reminds = db.Reminds.Where(x => $"{x.Date:dd.MM.yyyy HH}" == $"{DateTime.Now:dd.MM.yyyy HH}");
            foreach (var remind in reminds)
            {
                if (Utils.SendMessage(remind.VkID, remind.Text))
                {
                    db.Reminds.Remove(remind);
                }
                //TODO: else????
            }

            db.SaveChanges();
        }

        public void SendSchedule()
        {
            var grouped = db.Users.Where(x => x.Group != 0 && x.Schedule).GroupBy(x => x.Group);
            foreach (var group in grouped)
            {
                var ids = group.Select(x => x.Vk).ToList();
                Utils.SendMessage(ids, GetSchedule(DateTime.Today, group.Key)); //TODO: дополнить
            }
        }

        [NonAction]
        private string GetSchedule(DateTime date, short usergroup)
        {
            var result = $"Расписание на {date:dd.MM}:\n";
            string calen;
            using (var client = new WebClient())
            {
                try
                {
                    client.Encoding = Encoding.UTF8;
                    calen = client.DownloadString(
                        $"http://ruz.narfu.ru/?icalendar&oid={usergroup}&from={DateTime.Now:dd.MM.yyyy}");
                }
                catch (WebException e)
                {
                    return $"Какая-то ошибочка ({e.Message} - {e.Status}). Напиши @id***REMOVED*** (сюда) для решения проблемы!!";
                }
            }

            var calendar = Calendar.Load(calen);
            var events = calendar.Events.Where(x => x.Start.Date == date).Distinct().OrderBy(x => x.Start.Value).ToList();
            if (!events.Any()) return $"На {date:dd.MM} расписание отсутствует!";
            foreach (var ev in events)
            {
                var a = ev.Description.Split('\n');
                var time = a[0].Replace('п', ')');
                var group = a[1].Substring(3);
                var temp = a[5].Split('/');
                result += $"{time} - {a[2]} ({a[3]})\nУ группы {group}\n В аудитории {temp[1]} ({temp[0]})\n\n";
            }

            return result;
        }
    }
}