using System;
using System.Linq;
using FluentScheduler;
using Goblin.Helpers;
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
            db = new MainContext();
        }

        public void SendRemind()
        {
            //TODO: ?????
            var reminds = db.Reminds.Where(x => $"{x.Date.AddHours(-3):dd.MM.yyyy HH:mm}" == $"{DateTime.Now:dd.MM.yyyy HH:mm}");
            foreach (var remind in reminds)
            {
                //TODO: else????
                if (VkHelper.SendMessage(remind.VkID, remind.Text))
                {
                    db.Reminds.Remove(remind);
                }
            }

            db.SaveChanges();
        }
    }
}