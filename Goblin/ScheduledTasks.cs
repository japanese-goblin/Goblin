using FluentScheduler;
using Goblin.Helpers;
using System;
using System.Linq;
using System.Threading.Tasks;
using Goblin.Vk;

namespace Goblin
{
    public class ScheduledTasks : Registry
    {
        public ScheduledTasks()
        {
            //Schedule(() => SendSchedule()).ToRunEvery(0).Days().At(8, 0);
            //Schedule(() => SendSchedule()).ToRunEvery(0).Days().At(8, 0);
            Schedule(async () => await SendRemind()).ToRunEvery(1).Minutes();
        }

        public async Task SendRemind()
        {
            //TODO: ?????
            var reminds = DbHelper.Db.Reminds.Where(x => $"{x.Date:dd.MM.yyyy HH:mm}" == $"{DateTime.Now:dd.MM.yyyy HH:mm}");
            foreach (var remind in reminds)
            {
                await VkMethods.SendMessage(remind.VkID, $"Напоминаю:\n {remind.Text}");
                DbHelper.Db.Reminds.Remove(remind);
            }

            await DbHelper.Db.SaveChangesAsync();
        }
    }
}