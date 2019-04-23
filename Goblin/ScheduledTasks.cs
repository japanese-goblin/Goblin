using System;
using System.Linq;
using System.Threading.Tasks;
using Goblin.Persistence;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using Narfu;
using OpenWeatherMap;
using Vk;

namespace Goblin
{
    public class ScheduledTasks
    {
        private readonly MainContext _db;
        private readonly VkApi _api;
        private readonly WeatherInfo _weather;

        public ScheduledTasks(MainContext db, VkApi api, WeatherInfo weather)
        {
            _db = db;
            _api = api;
            _weather = weather;

            InitJobs();
        }

        public async Task SendRemind()
        {
            var reminds =
                _db.Reminds
                   .AsNoTracking()
                   .Where(x => x.Date.ToString("dd.MM.yyyy HH:mm") == DateTime.Now.ToString("dd.MM.yyyy HH:mm"))
                   .ToArray();

            if(!reminds.Any())
            {
                return;
            }

            foreach(var remind in reminds)
            {
                await _api.Messages.Send(remind.VkId, $"Напоминаю:\n{remind.Text}");
                _db.Reminds.Remove(remind);
            }

            await _db.SaveChangesAsync();
        }

        public async Task SendSchedule()
        {
            await Task.Factory.StartNew(async () =>
            {
                var grouped = _db.GetScheduleUsers().GroupBy(x => x.Group);
                foreach(var group in grouped)
                {
                    var ids = group.Select(x => x.Vk).ToArray();
                    var schedule = await StudentsSchedule.GetScheduleAtDate(DateTime.Today, group.Key);
                    await _api.Messages.Send(ids, schedule);
                    await Task.Delay(700); //TODO: потому что сайт выдает 404
                }
            });
        }

        public async Task SendWeather()
        {
            await Task.Factory.StartNew(async () =>
            {
                var grouped = _db.GetWeatherUsers().GroupBy(x => x.City);
                foreach(var group in grouped)
                {
                    var ids = group.Select(x => x.Vk).ToArray();
                    await _api.Messages.Send(ids, await _weather.GetWeather(group.Key));
                    await Task.Delay(300);
                }
            });
        }

        public async Task SendToConv(int id, int group = 0, string city = "")
        {
            const int convId = 2000000000;
            id = convId + id;

            if(!string.IsNullOrEmpty(city) && await _weather.CheckCity(city))
            {
                var weather = await _weather.GetWeather(city);
                await _api.Messages.Send(id, weather);
            }

            if(StudentsSchedule.IsCorrectGroup(group))
            {
                var schedule = await StudentsSchedule.GetScheduleAtDate(DateTime.Now, group);
                await _api.Messages.Send(id, schedule);
            }
        }

        public void InitJobs()
        {
            // минуты часи дни месяцы дни-недели
            RecurringJob.AddOrUpdate<ScheduledTasks>("SendRemind", x => x.SendRemind(), Cron.Minutely,
                                                     TimeZoneInfo.Local);

            RecurringJob.AddOrUpdate<ScheduledTasks>("SendSchedule", x => x.SendSchedule(),
                                                     "0 6 * * 1-6", TimeZoneInfo.Local);

            RecurringJob.AddOrUpdate<ScheduledTasks>("SendWeather", x => x.SendWeather(),
                                                     "0 7 * * *", TimeZoneInfo.Local);

            foreach(var job in _db.Jobs)
            {
                RecurringJob.AddOrUpdate<ScheduledTasks>(
                    $"DAILY__{job.JobName}",
                    x => x.SendToConv(job.Conversation, job.NarfuGroup, job.WeatherCity),
                    $"{job.Minutes} {job.Hours} * * 1-6", TimeZoneInfo.Local
                );
            }
        }

        public void Dummy()
        {

        }
    }
}