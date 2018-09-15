using System;
using System.Linq;
using System.Threading.Tasks;
using FluentScheduler;
using Goblin.Helpers;
using Goblin.Models;

namespace Goblin
{
    public class ScheduledTasks : Registry
    {
        private readonly MainContext db;

        public ScheduledTasks()
        {
            //Schedule(() => SendSchedule()).ToRunEvery(0).Days().At(8, 0);
            //Schedule(() => SendSchedule()).ToRunEvery(0).Days().At(8, 0);
            Schedule(async () => await SendRemind()).ToRunEvery(1).Minutes();

            db = new MainContext();
        }

        public async Task SendRemind()
        {
            //TODO: ?????
            var reminds = db.Reminds.Where(x => $"{x.Date:dd.MM.yyyy HH:mm}" == $"{DateTime.Now:dd.MM.yyyy HH:mm}");
            foreach (var remind in reminds)
            {
                if (await VkHelper.SendMessage(remind.VkID, remind.Text))
                {
                    db.Reminds.Remove(remind);
                }
            }

            await db.SaveChangesAsync();
        }
    }
}