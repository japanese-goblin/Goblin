using System;
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

            BackgroundJob.Enqueue<SendRemindTask>(x => x.SendOldReminds());

            RecurringJob.AddOrUpdate<SendRemindTask>("SendRemind", x => x.SendRemindEveryMinute(),
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
            foreach(var job in _db.CronJobs)
            {
                RecurringJob.AddOrUpdate<SendToChatTasks>(
                                                          $"DAILY__{job.Name}__{job.ConsumerType}",
                                                          x => x.SendToConv(job.ChatId, job.NarfuGroup,
                                                                            job.WeatherCity, job.ConsumerType),
                                                          $"{job.Minutes} {job.Hours} * * *",
                                                          TimeZoneInfo.Local
                                                         );
            }
        }
    }
}