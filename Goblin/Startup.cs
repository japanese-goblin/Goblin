using Goblin.Bot;
using Goblin.Filters;
using Goblin.Persistence;
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
using System;
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
            if (env.IsDevelopment())
            {
                builder.AddUserSecrets<Startup>();
            }

            Configuration = builder.Build();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            var connectionString = Configuration.GetConnectionString("DefaultConnection");

            services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(connectionString));

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
            if (env.IsDevelopment())
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
            app.UseHangfireDashboard("/Admin/HangFire", new DashboardOptions
            {
                Authorization = new[] { new AuthFilter() },
                AppPath = "/Admin/",
                StatsPollingInterval = 10000,
                DisplayStorageConnectionString = false
            });

            app.UseMvcWithDefaultRoute();

            ConfigureJobs();
        }

        private void ConfigureJobs()
        {
            BackgroundJob.Enqueue<ScheduledTasks>(x => x.Dummy()); //TODO:
        }
    }
}