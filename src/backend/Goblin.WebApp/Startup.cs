using AutoMapper;
using Goblin.Application.Core;
using Goblin.Application.Telegram;
using Goblin.Application.Vk;
using Goblin.BackgroundJobs.Jobs;
using Goblin.DataAccess;
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

namespace Goblin.WebApp;

public class Startup
{
    private IConfiguration Configuration { get; }
    private readonly IWebHostEnvironment _env;

    public Startup(IWebHostEnvironment env)
    {
        _env = env;
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
        services.AddVkLayer(Configuration);
        services.AddTelegramLayer(Configuration);
        services.AddHangfire(config => { config.UseMemoryStorage(); });

        var mappingConfig = new MapperConfiguration(mc => { mc.AddProfile(new MappingProfile()); });
        var mapper = mappingConfig.CreateMapper();
        services.AddSingleton(mapper);

        services.AddAuth(Configuration);

        if(_env.IsDevelopment())
        {
            services.AddControllersWithViews()
                    .AddRazorRuntimeCompilation()
                    .AddNewtonsoftJson();
        }
        else
        {
            services.AddControllersWithViews()
                    .AddNewtonsoftJson();
        }

        services.AddRazorPages();
        services.AddDatabaseDeveloperPageExceptionFilter();
        services.AddHangfireServer(x =>
        {
            x.WorkerCount = 4;
        });
        GlobalJobFilters.Filters.Add(new AutomaticRetryAttribute { Attempts = 0 });
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if(env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
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

        app.UseHangfireDashboard("/Admin/HangFire", new DashboardOptions
        {
            Authorization = new[] { new AuthFilter() },
            AppPath = "/",
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

        BackgroundJob.Enqueue<StartupTasks>(x => x.ConfigureHangfire());
        BackgroundJob.Enqueue<StartupTasks>(x => x.RemoveInactiveUsersFromVk());
        BackgroundJob.Enqueue<ResetUsersGroups>(x => x.Execute());
    }
}