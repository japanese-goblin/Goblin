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

        }

        public void SendSchedule()
        {
            SendScheduleToUsers();
        }

        private void SendScheduleToUsers()
        {
            var grouped = db.Users.Where(x => x.CityNumber == 0).GroupBy(x => x.Group);
            foreach (var group in grouped)
            {
                var ids = group.Select(x => x.Vk).ToList();
                Utils.SendMessage(ids, "расписание"); //TODO: дополнить
            }
        }

        private void SendWeatherToUsers()
        {
            var grouped = db.Users.Where(x => x.Group == 0).GroupBy(x => x.City);
            foreach (var group in grouped)
            {
                var ids = group.Select(x => x.Vk).ToList();
                Utils.SendMessage(ids, "погода"); //TODO: дополнить
            }
        }
    }
}