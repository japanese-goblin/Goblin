using System;
using System.Linq;
using FluentScheduler;
using Goblin.Models;
using Microsoft.EntityFrameworkCore;

namespace Goblin
{
    public class ScheduledTasks : Registry
    {
        private readonly MainContext db;

        public ScheduledTasks()
        {
            Schedule(() => SendSchedule()).ToRunEvery(1).Days().At(8, 0);
            Schedule(() => SendWeather()).ToRunEvery(1).Days().At(9, 0);
            Schedule(() => SendRemind()).ToRunEvery(1).Minutes();
            //TODO: ЭТО ЧТО ВООБЩЕ ТАКОЕ??7?7??
            var a = new DbContextOptionsBuilder<MainContext>();
            a.UseNpgsql("***REMOVED***");
            db = new MainContext(a.Options);
        }

        public void SendWeather()
        {
            Console.WriteLine("Отправка погоды...");
            var grouped = Utils.DB.Users.Where(x => x.CityNumber != 0 && x.Weather).GroupBy(x => x.City);
            foreach (var group in grouped)
            {
                var ids = group.Select(x => x.Vk).ToList();
                Utils.SendMessage(ids, $"В городе {group.Key} очень хорошая погода!"); //TODO: дополнить
            }
        }

        public void SendRemind()
        {
            Console.WriteLine("Отправка напоминалок...");
            //TODO: ?????
            var reminds = db.Reminds.Where(x => $"{x.Date:dd.MM.yyyy HH:mm}" == $"{DateTime.Now.AddHours(-3):dd.MM.yyyy HH:mm}");
            foreach (var remind in reminds)
            {
                //TODO: else????
                if (Utils.SendMessage(remind.VkID, remind.Text))
                {
                    db.Reminds.Remove(remind);
                }
            }

            db.SaveChanges();
        }

        public void SendSchedule()
        {
            Console.WriteLine("Рассылка расписания...");
            var grouped = Utils.DB.Users.Where(x => x.Group != 0 && x.Schedule).GroupBy(x => x.Group);
            foreach (var group in grouped)
            {
                Console.WriteLine($"Отправка для группы {group.Key}");
                var ids = group.Select(x => x.Vk).ToList();
                Utils.SendMessage(ids, Utils.GetSchedule(DateTime.Today, group.Key)); //TODO: дополнить
            }
        }
    }
}