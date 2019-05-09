using System;
using System.Globalization;
using System.Reflection;
using Goblin.Bot;
using Goblin.Bot.Notifications.GroupJoin;
using Goblin.Domain.Entities;
using Goblin.Persistence;
using Goblin.WebUI.Extensions;
using Goblin.WebUI.Filters;
using Goblin.WebUI.Hangfire;
using Hangfire;
using Hangfire.MemoryStorage;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenWeatherMap;
using Vk;

namespace Goblin.WebUI
{
    public class Startup
    {
        private IConfiguration Configuration { get; }

        public Startup(IHostingEnvironment env)
        {
            var culture = new CultureInfo("ru-RU");
            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;

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
            services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(connectionString));

            services.AddHangfire(config => { config.UseMemoryStorage(); });

            services.AddScoped<Handler>();
            services.AddScoped<CommandExecutor>();
            services.AddBotCommands();

            services.AddSingleton(x => new VkApi(Configuration["Config:Vk_Token"]));
            services.AddSingleton(x => new WeatherInfo(Configuration["Config:OWM_Token"]));

            services.AddIdentity<SiteUser, IdentityRole>()
                    .AddRoles<IdentityRole>()
                    .AddDefaultUI(UIFramework.Bootstrap4)
                    .AddEntityFrameworkStores<ApplicationDbContext>();

            services.AddAuthentication()
                    .AddVkontakte(options =>
                    {
                        options.ApiVersion = "5.8";
                        options.ClientId = Configuration["VkAuth:AppId"];
                        options.ClientSecret = Configuration["VkAuth:AppSecret"];
                        options.Scope.Add("email");
                    });

            services.AddHttpsRedirection(options => { options.HttpsPort = 443; });
            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.KnownNetworks.Clear();
                options.KnownProxies.Clear();
                options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            });

            services.AddMediatR(typeof(GroupJoinNotification).GetTypeInfo().Assembly);

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if(env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseHsts();
                app.UseForwardedHeaders();
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

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                                "default",
                                "{controller=Home}/{action=Index}/{id?}");
            });

            ConfigureJobs();
        }

        private void ConfigureJobs()
        {
            BackgroundJob.Enqueue<ScheduledTasks>(x => x.Dummy()); //TODO:
            BackgroundJob.Enqueue<CreateRolesTask>(x => x.CreateRoles());
        }
    }
}