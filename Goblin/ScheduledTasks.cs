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
            //Schedule(() => SendSchedule()).ToRunEvery(0).Days().At(8, 0);
            //Schedule(() => SendSchedule()).ToRunEvery(0).Days().At(8, 0);
            Schedule(() => SendRemind()).ToRunEvery(1).Minutes();
            //TODO: ЭТО ЧТО ВООБЩЕ ТАКОЕ??7?7??
            var a = new DbContextOptionsBuilder<MainContext>();
            a.UseNpgsql("***REMOVED***");
            db = new MainContext(a.Options);
        }

        public void SendRemind()
        {
            //TODO: ?????
            var reminds = db.Reminds.Where(x => $"{x.Date.AddHours(-3):dd.MM.yyyy HH:mm}" == $"{DateTime.Now:dd.MM.yyyy HH:mm}");
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
    }
}