using System;
using Goblin.Bot;
using Goblin.Data.Models;
using Hangfire;
using Hangfire.MemoryStorage;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenWeatherMap;
using Vk;

namespace Goblin
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var connectionString = Configuration.GetConnectionString("DefaultConnection");

            services.AddDbContext<MainContext>(options => options.UseNpgsql(connectionString));

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
                    {
                        options.AccessDeniedPath = new PathString("/Admin/");
                        options.LoginPath = new PathString("/Admin/Login");
                    });

            services.AddHangfire(config => { config.UseMemoryStorage(); });

            services.AddScoped<Handler>();
            services.AddScoped<CommandExecutor>();
            services.AddBotCommands();

            services.AddSingleton(x => new VkApi(Configuration["Config:AccessToken"]));
            services.AddSingleton(x => new WeatherInfo(Configuration["Config:OWMToken"]));

            services.AddMvc()
                    .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if(env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();
            app.UseAuthentication();

            var options = new BackgroundJobServerOptions { WorkerCount = Environment.ProcessorCount * 2 };
            app.UseHangfireServer(options);

            //app.UseHttpsRedirection(); //TODO
            app.UseMvcWithDefaultRoute();

            ConfigureJobs();
        }

        private void ConfigureJobs()
        {
            // минуты часи дни месяцы дни-недели
            RecurringJob.AddOrUpdate<ScheduledTasks>(x => x.SendRemind(), Cron.Minutely,
                                                     TimeZoneInfo.Local);
            RecurringJob.AddOrUpdate<ScheduledTasks>(x => x.SendSchedule(), "0 6 * * 1-6",
                                                     TimeZoneInfo.Local);
            RecurringJob.AddOrUpdate<ScheduledTasks>(x => x.SendWeather(), "0 7 * * *",
                                                     TimeZoneInfo.Local);
            //TODO вынести в бд
            RecurringJob.AddOrUpdate<ScheduledTasks>("igor",
                                                     x => x.SendToConv(5, 351616, ""),
                                                     "05 6 * * 1-6", TimeZoneInfo.Local);
            RecurringJob.AddOrUpdate<ScheduledTasks>("pesi",
                                                     x => x.SendToConv(3, 351617, "Архангельск"),
                                                     "15 6 * * 1-6", TimeZoneInfo.Local);
        }
    }
}