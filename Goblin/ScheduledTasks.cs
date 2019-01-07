using FluentScheduler;
using Goblin.Helpers;
using Narfu;
using OpenWeatherMap;
using System;
using System.Linq;
using System.Threading.Tasks;
using Vk;

namespace Goblin
{
    public class ScheduledTasks : Registry
    {
        public ScheduledTasks()
        {
            Schedule(async () => await SendRemind()).ToRunEvery(1).Minutes();
            //Schedule(async () => await SendSchedule()).ToRunEvery(0).Days().At(6, 0);
            //Schedule(async () => await SendWeather()).ToRunEvery(0).Days().At(7, 0);
            //Schedule(async () => await SendToConv(5, 351616)).ToRunEvery(0).Days().At(6, 05); // IGOR
            Schedule(async () => await SendToConv(2, 351617)).ToRunEvery(0).Days().At(6, 15); // MY
        }

        private async Task SendRemind()
        {
            //TODO: ?????
            var reminds = DbHelper.Db.Reminds.Where(x => $"{x.Date:dd.MM.yyyy HH:mm}" == $"{DateTime.Now:dd.MM.yyyy HH:mm}");
            if (!reminds.Any()) return;

            foreach (var remind in reminds)
            {
                await Api.Messages.Send(remind.VkID, $"Напоминаю:\n {remind.Text}");
                DbHelper.Db.Reminds.Remove(remind);
            }

            await DbHelper.Db.SaveChangesAsync();
        }

        private async Task SendSchedule()
        {
            if (DateTime.Now.DayOfWeek == DayOfWeek.Sunday) return;

            await Task.Factory.StartNew(async () =>
            {
                var grouped = DbHelper.GetScheduleUsers().GroupBy(x => x.Group);
                foreach (var group in grouped)
                {
                    var ids = group.Select(x => x.Vk).ToArray();
                    var schedule = await StudentsSchedule.GetScheduleAtDate(DateTime.Today, group.Key);
                    await Api.Messages.Send(ids, schedule);
                    await Task.Delay(500); //TODO - 3 запроса в секунду
                }
            });
        }

        private async Task SendWeather()
        {
            await Task.Factory.StartNew(async () =>
            {
                var grouped = DbHelper.GetWeatherUsers().GroupBy(x => x.City);
                foreach (var group in grouped)
                {
                    var ids = group.Select(x => x.Vk).ToArray();
                    await Api.Messages.Send(ids, await WeatherInfo.GetWeather(group.Key));
                    await Task.Delay(700); //TODO - 3 запроса в секунду
                }
            });
        }

        private async Task SendToConv(int id, int group = 0, string city = "")
        {
            if (!StudentsSchedule.IsCorrectGroup(group))
            {
                return;
            }

            id = 2000000000 + id;

            var schedule = await StudentsSchedule.GetScheduleAtDate(DateTime.Now, group);
            await Api.Messages.Send(id, schedule);

            if (!string.IsNullOrEmpty(city) && await WeatherInfo.CheckCity(city))
            {
                var weather = await WeatherInfo.GetWeather(city);
                await Api.Messages.Send(id, weather);
            }
        }
    }
}