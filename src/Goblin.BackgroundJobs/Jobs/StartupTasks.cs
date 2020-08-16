using System;
using System.Linq;
using Goblin.Application.Core.Options;
using Goblin.DataAccess;
using Hangfire;
using Microsoft.Extensions.Options;

namespace Goblin.BackgroundJobs.Jobs
{
    public class StartupTasks
    {
        private readonly BotDbContext _db;
        private readonly MailingOptions _options;

        public StartupTasks(BotDbContext db, IOptions<MailingOptions> options)
        {
            _db = db;
            _options = options.Value;

            InitJobsFromDatabase();
        }

        public void ConfigureHangfire()
        {
            ConfigureMailing();

            BackgroundJob.Enqueue<SendRemindTasks>(x => x.SendOldRemindsOnStartup());

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
                RecurringJob.AddOrUpdate<ScheduleTask>("DailySchedule", x => x.Execute(),
                                                       scheduleTime, TimeZoneInfo.Local);
            }

            if(weatherSettings.IsEnabled)
            {
                var weatherTime = $"{weatherSettings.Minute} {weatherSettings.Hour} * * *";
                RecurringJob.AddOrUpdate<WeatherTask>("DailyWeather", x => x.Execute(),
                                                      weatherTime, TimeZoneInfo.Local);
            }
        }

        private void InitJobsFromDatabase()
        {
            foreach(var job in _db.CronJobs.ToArray())
            {
                RecurringJob.AddOrUpdate<SendToChatTasks>(
                                                          $"{job.ConsumerType}__{job.Name}",
                                                          x => x.Execute(job.ChatId, job.ConsumerType, job.CronType,
                                                                         job.WeatherCity, job.NarfuGroup, job.Text),
                                                          job.Time.ToString(),
                                                          TimeZoneInfo.Local
                                                         );
            }
        }
    }
}