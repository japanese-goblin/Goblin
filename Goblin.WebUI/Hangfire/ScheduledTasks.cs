using System;
using System.Linq;
using System.Threading.Tasks;
using Goblin.Persistence;
using Goblin.WebUI.Extensions;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using Narfu;
using OpenWeatherMap;
using Vk;

namespace Goblin.WebUI.Hangfire
{
    public class ScheduledTasks
    {
        private readonly ApplicationDbContext _db;
        private readonly VkApi _api;
        private readonly WeatherInfo _weather;

        private const int ChunkLimit = 100;

        public ScheduledTasks(ApplicationDbContext db, VkApi api, WeatherInfo weather)
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
                   .Where(x => x.Date.ToString("dd.MM.yyyy HH:mm") == DateTime.Now.ToString("dd.MM.yyyy HH:mm")); //TODO: что-то сделать с датой

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

        public void SendDailyStuff()
        {
            var weatherJob = BackgroundJob.Enqueue(() => SendWeather());
            if (DateTime.Now.DayOfWeek != DayOfWeek.Sunday)
            {
                BackgroundJob.ContinueWith(weatherJob, () => SendSchedule());
            }
        }

        public async Task SendSchedule()
        {
            var grouped = _db.GetScheduleUsers().GroupBy(x => x.Group);
            foreach(var group in grouped)
            {
                foreach(var chunk in group.Chunk(ChunkLimit))
                {
                    var ids = chunk.Select(x => x.Vk);
                    var schedule = await StudentsSchedule.GetScheduleAtDate(DateTime.Today, group.Key);
                    BackgroundJob.Enqueue(() => _api.Messages.Send(ids, schedule, null, null));
                }   
            }
        }

        public async Task SendWeather()
        {
            var grouped = _db.GetWeatherUsers().GroupBy(x => x.City);
            foreach(var group in grouped)
            {
                foreach(var chunk in group.Chunk(ChunkLimit))
                {
                    var ids = chunk.Select(x => x.Vk);
                    var weather = await _weather.GetDailyWeatherString(group.Key, DateTime.Today);
                    BackgroundJob.Enqueue(() => _api.Messages.Send(ids, weather, null, null));
                }
            }
        }

        public async Task SendToConv(int id, int group = 0, string city = "")
        {
            const int convId = 2000000000;
            id = convId + id;
            var x = id >= 0;

            if(!string.IsNullOrWhiteSpace(city) && await _weather.CheckCity(city))
            {
                var weather = await _weather.GetDailyWeatherString(city, DateTime.Today);
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

            RecurringJob.AddOrUpdate<ScheduledTasks>("SendDailyStuff", x => x.SendDailyStuff(),
                                                     "30 5 * * *", TimeZoneInfo.Local);

            //RecurringJob.AddOrUpdate<ScheduledTasks>("SendSchedule", x => x.SendSchedule(),
            //                                         "0 6 * * 1-6", TimeZoneInfo.Local);

            //RecurringJob.AddOrUpdate<ScheduledTasks>("SendWeather", x => x.SendWeather(),
            //                                         "0 7 * * *", TimeZoneInfo.Local);

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
            //TODO: lol
        }
    }
}