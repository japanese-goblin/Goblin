using System;
using Goblin.Application.Hangfire;
using Goblin.WebApp.Filters;
using Hangfire;
using Microsoft.AspNetCore.Builder;

namespace Goblin.WebApp.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static void UseDashboard(this IApplicationBuilder app)
        {
            var options = new BackgroundJobServerOptions { WorkerCount = 4 };
            app.UseHangfireServer(options);
            app.UseHangfireDashboard("/Admin/HangFire", new DashboardOptions
            {
                Authorization = new[] { new AuthFilter() },
                AppPath = "/Admin/",
                StatsPollingInterval = 10000,
                DisplayStorageConnectionString = false
            });
        }

        public static void AddHangfireJobs(this IApplicationBuilder services)
        {
            BackgroundJob.Enqueue<CreateRoleTask>(x => x.CreateRoles());
            BackgroundJob.Enqueue<SendToConversationTasks>(x => x.Dummy());
            RecurringJob.AddOrUpdate<SendRemindTask>("SendRemind", x => x.SendRemind(), Cron.Minutely,
                                                     TimeZoneInfo.Local);
            RecurringJob.AddOrUpdate<ScheduleTask>("SendDailySchedule", x => x.SendSchedule(),
                                                   "05 6 * * 1-6", TimeZoneInfo.Local);
            RecurringJob.AddOrUpdate<WeatherTask>("SendDailyWeather", x => x.SendDailyWeather(),
                                                  "25 5 * * *", TimeZoneInfo.Local);
        }
    }
}