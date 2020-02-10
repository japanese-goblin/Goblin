using System;
using Goblin.Application;
using Goblin.Application.Hangfire;
using Goblin.DataAccess;
using Goblin.WebApp.Extensions;
using Goblin.WebApp.Filters;
using Goblin.WebApp.HostedServices;
using Hangfire;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Goblin.WebApp
{
    public class Startup
    {
        private IConfiguration Configuration { get; }

        public Startup(IWebHostEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                          .SetBasePath(env.ContentRootPath)
                          .AddJsonFile("appsettings.json", false, true)
                          .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true)
                          .AddEnvironmentVariables();
            if(env.IsDevelopment())
            {
                builder.AddUserSecrets<Startup>();
            }

            Configuration = builder.Build();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHostedService<MigrationHostedService>();
            services.AddHostedService<CreateDefaultRolesHostedService>();

            services.AddDataAccessLayer(Configuration);
            services.AddApplication(Configuration);

            services.AddAuth(Configuration);

            services.AddControllersWithViews()
                    .AddNewtonsoftJson();
            services.AddRazorPages();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if(env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseHangfireServer(new BackgroundJobServerOptions { WorkerCount = 4 });
            app.UseHangfireDashboard("/Admin/HangFire", new DashboardOptions
            {
                Authorization = new[] { new AuthFilter() },
                AppPath = "/Admin/",
                StatsPollingInterval = 10000,
                DisplayStorageConnectionString = false
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                                             "Areas",
                                             "{area:exists}/{controller=Home}/{action=Index}/{id?}");
                endpoints.MapDefaultControllerRoute();
                endpoints.MapRazorPages();
            });

            ConfigureHangfireJobs();
        }

        private void ConfigureHangfireJobs()
        {
            BackgroundJob.Enqueue<SendToConversationTasks>(x => x.Dummy());
            RecurringJob.AddOrUpdate<SendRemindTask>("SendRemind", x => x.SendRemind(),
                                                     Cron.Minutely, TimeZoneInfo.Local);
            RecurringJob.AddOrUpdate<ScheduleTask>("SendDailySchedule", x => x.SendSchedule(),
                                                   "05 6 * * 1-6", TimeZoneInfo.Local);
            RecurringJob.AddOrUpdate<WeatherTask>("SendDailyWeather", x => x.SendDailyWeather(),
                                                  "30 6 * * *", TimeZoneInfo.Local);

            BackgroundJob.Enqueue<StartupTasks>(x => x.SendOldReminds());
        }
    }
}