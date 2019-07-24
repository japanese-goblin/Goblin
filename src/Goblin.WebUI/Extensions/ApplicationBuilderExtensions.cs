using System;
using Goblin.WebUI.Filters;
using Hangfire;
using Microsoft.AspNetCore.Builder;

namespace Goblin.WebUI.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static void UseDashboard(this IApplicationBuilder app)
        {
            var options = new BackgroundJobServerOptions { WorkerCount = Environment.ProcessorCount * 2 };
            app.UseHangfireServer(options);
            app.UseHangfireDashboard("/Admin/HangFire", new DashboardOptions
            {
                Authorization = new[] { new AuthFilter() },
                AppPath = "/Admin/",
                StatsPollingInterval = 10000,
                DisplayStorageConnectionString = false
            });
        }
    }
}