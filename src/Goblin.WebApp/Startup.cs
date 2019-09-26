using Goblin.Narfu;
using Goblin.OpenWeatherMap;
using Goblin.WebApp.Extensions;
using Goblin.WebApp.Filters;
using Goblin.WebApp.HostedServices;
using Hangfire;
using Hangfire.MemoryStorage;
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

            services.AddDbContexts(Configuration);
            services.AddOptions(Configuration);

            services.AddHangfire(config => { config.UseMemoryStorage(); });

            services.AddVkApi(Configuration);
            services.AddSingleton(x =>
            {
                var api = new OpenWeatherMapApi(Configuration["OWM:AccessToken"]);
                return api;
            });
            services.AddSingleton<NarfuApi>();

            services.AddBotFeatures();

            services.AddAuth(Configuration);

            services.AddResponseCaching();

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

            var options = new BackgroundJobServerOptions { WorkerCount = 4 };
            app.UseHangfireServer(options);
            app.UseHangfireDashboard("/Admin/HangFire", new DashboardOptions
            {
                Authorization = new[] { new AuthFilter() },
                AppPath = "/Admin/",
                StatsPollingInterval = 10000,
                DisplayStorageConnectionString = false
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
                endpoints.MapRazorPages();
            });
        }
    }
}