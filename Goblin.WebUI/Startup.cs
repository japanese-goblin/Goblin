using System.Globalization;
using System.Reflection;
using Goblin.Bot.Notifications.GroupJoin;
using Goblin.Persistence;
using Goblin.WebUI.Extensions;
using Goblin.WebUI.Hangfire;
using Hangfire;
using Hangfire.MemoryStorage;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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

            services.AddBotFeatures();

            services.AddAdditions(Configuration);

            services.AddAuth(Configuration);

            services.AddHttpsRedirect();

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

            app.UseDashboard();

            app.UseMvcWithDefaultRoute();

            ConfigureJobs();
        }

        private void ConfigureJobs()
        {
            BackgroundJob.Enqueue<ScheduledTasks>(x => x.Dummy()); //TODO:
            BackgroundJob.Enqueue<CreateRolesTask>(x => x.CreateRoles());
        }
    }
}