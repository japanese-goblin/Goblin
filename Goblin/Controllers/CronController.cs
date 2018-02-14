using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Goblin.Models;
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
            SendWeatherToUsers();
        }

        public void SendRemind()
        {
            SendRemindsToUsers();
        }

        public void SendSchedule()
        {
            SendScheduleToUsers();
        }

        [NonAction]
        private void SendScheduleToUsers()
        {
            var grouped = db.Users.Where(x => x.Group != 0 && x.Schedule).GroupBy(x => x.Group);
            foreach (var group in grouped)
            {
                var ids = group.Select(x => x.Vk).ToList();
                Utils.SendMessage(ids, "расписание"); //TODO: дополнить
            }
        }

        [NonAction]
        private void SendWeatherToUsers()
        {
            var grouped = db.Users.Where(x => x.CityNumber != 0 && x.Weather).GroupBy(x => x.City);
            foreach (var group in grouped)
            {
                var ids = group.Select(x => x.Vk).ToList();
                Utils.SendMessage(ids, "погода"); //TODO: дополнить
            }
        }

        [NonAction]
        private void SendRemindsToUsers()
        {
            var reminds = db.Reminds.Where(x => $"{x.Date.AddHours(3):dd.MM.yyyy HH}" == $"{DateTime.Now:dd.MM.yyyy HH}");
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

        //[NonAction]
        //private DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        //{
        //    // Unix timestamp is seconds past epoch
        //    System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
        //    dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
        //    return dtDateTime;
        //}
    }
}