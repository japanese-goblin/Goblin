using System;
using System.Linq;
using Goblin.Application.Core.Options;
using Goblin.DataAccess;
using Hangfire;
using Microsoft.Extensions.Options;

namespace Goblin.WebApp.Hangfire
{
    public class StartupTasks
    {
        private readonly BotDbContext _db;
        private readonly MailingOptions _options;

        public StartupTasks(BotDbContext db, IOptions<MailingOptions> options)
        {
            _db = db;
            _options = options.Value;

            InitJobs();
        }

        public void ConfigureHangfire()
        {
            ConfigureMailing();

            BackgroundJob.Enqueue<SendRemindTasks>(x => x.SendOldReminds());

            RecurringJob.AddOrUpdate<SendRemindTasks>("SendRemind", x => x.SendRemindEveryMinute(),
                                                      Cron.Minutely, TimeZoneInfo.Local);
        }

        private void ConfigureMailing()
        {
            var scheduleSettings = _options.Schedule;
            var weatherSettings = _options.Weather;

            if(scheduleSettings.IsEnabled)
            {
                var scheduleTime = $"{scheduleSettings.Minute} {scheduleSettings.Hour} * * 1-6";
                RecurringJob.AddOrUpdate<ScheduleTask>("DailySchedule", x => x.SendSchedule(),
                                                       scheduleTime, TimeZoneInfo.Local);
            }

            if(weatherSettings.IsEnabled)
            {
                var weatherTime = $"{weatherSettings.Minute} {weatherSettings.Hour} * * *";
                RecurringJob.AddOrUpdate<WeatherTask>("DailyWeather", x => x.SendDailyWeather(),
                                                      weatherTime, TimeZoneInfo.Local);
            }
        }

        public void InitJobs()
        {
            foreach(var job in _db.CronJobs.ToArray())
            {
                RecurringJob.AddOrUpdate<SendToChatTasks>(
                                                          $"DAILY__{job.Name}__{job.ConsumerType}",
                                                          x => x.SendToConv(job.ChatId, job.ConsumerType, job.CronType,
                                                                            job.WeatherCity, job.NarfuGroup, job.Text),
                                                          job.Time.ToString(),
                                                          TimeZoneInfo.Local
                                                         );
            }
        }
    }
}