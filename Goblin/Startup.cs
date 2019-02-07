using System;
using Goblin.Bot;
using Goblin.Bot.Commands;
using Goblin.Models;
using Hangfire;
using Hangfire.MemoryStorage;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenWeatherMap;
using Vk;
using Random = Goblin.Bot.Commands.Random;

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

            services.AddHangfire(config =>
            {
                config.UseMemoryStorage();
            });

            services.AddScoped<Handler>();
            services.AddScoped<CommandExecutor>();

            services.AddScoped<ICommand, AddRemind>();
            services.AddScoped<ICommand, Debug>();
            services.AddScoped<ICommand, Exams>();
            services.AddScoped<ICommand, FindTeacher>();
            services.AddScoped<ICommand, GetReminds>();
            services.AddScoped<ICommand, KeyboardCommand>(); //TODO
            services.AddScoped<ICommand, Quote>();
            services.AddScoped<ICommand, Random>();
            services.AddScoped<ICommand, Schedule>();
            services.AddScoped<ICommand, SendAdmin>();
            services.AddScoped<ICommand, SetCity>();
            services.AddScoped<ICommand, SetGroup>();
            services.AddScoped<ICommand, SetMailing>();
            services.AddScoped<ICommand, TeacherSchedule>();
            services.AddScoped<ICommand, UnsetMailing>();
            services.AddScoped<ICommand, Weather>();
            services.AddScoped<ICommand, Help>();

            services.AddSingleton(x => new VkApi(Configuration["Config:AccessToken"]));
            services.AddSingleton(x => new WeatherInfo(Configuration["Config:OWMToken"]));

            //JobManager.Initialize(new ScheduledTasks(services.get)); // TODO

            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if(env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();
            app.UseAuthentication();

            var options = new BackgroundJobServerOptions { WorkerCount = Environment.ProcessorCount * 2 };
            app.UseHangfireServer(options);
            app.UseHangfireDashboard("/sched");

            //app.UseHttpsRedirection(); //TODO
            app.UseMvc(routes =>
            {
                routes.MapRoute(name: "default",
                                template: "{controller=Home}/{action=Index}/{id?}");
            });

            ConfigureJobs();
        }

        private void ConfigureJobs()
        {
            //    Schedule(async () => await SendRemind()).ToRunEvery(1).Minutes();
            //    Schedule(async () => await SendSchedule()).ToRunEvery(0).Days().At(6, 0);
            //    Schedule(async () => await SendWeather()).ToRunEvery(0).Days().At(7, 0);
            //    //TODO вынести в бд
            //    Schedule(async () => await SendToConv(5, 351616)).ToRunEvery(0).Days().At(6, 05); // IGOR
            //    Schedule(async () => await SendToConv(3, 351617, "Архангельск")).ToRunEvery(0).Days().At(6, 15); // MY

            RecurringJob.AddOrUpdate<ScheduledTasks>(x => x.SendRemind(), "* * * * *",
                                                     TimeZoneInfo.Local);
            RecurringJob.AddOrUpdate<ScheduledTasks>(x => x.SendSchedule(), "0 6 * * 0-6",
                                                     TimeZoneInfo.Local); //TODO: check
            RecurringJob.AddOrUpdate<ScheduledTasks>(x => x.SendWeather(), "0 7 * * *",
                                                     TimeZoneInfo.Local); //TODO: check
            //TODO вынести в бд
            RecurringJob.AddOrUpdate<ScheduledTasks>("igor",
                                                     x => x.SendToConv(5, 351616, ""),
                                                     "05 6 * * 0-6", TimeZoneInfo.Local); //TODO: check
            RecurringJob.AddOrUpdate<ScheduledTasks>("pesi",
                                                     x => x.SendToConv(3, 351617, "Архангельск"),
                                                     "15 6 * * 0-6", TimeZoneInfo.Local); //TODO: check
        }
    }
}
