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
        public IConfiguration Configuration { get; }

        public Startup(IHostingEnvironment env)
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

            services.AddSingleton(x => new VkApi(Configuration["Config:Vk_Token"]));
            services.AddSingleton(x => new WeatherInfo(Configuration["Config:OWM_Token"]));

            services.AddMvc()
                    .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if(env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();
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