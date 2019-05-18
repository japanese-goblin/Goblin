using System;
using System.Net.Http;
using Flurl.Http;
using Goblin.Bot;
using Goblin.Bot.Commands;
using Goblin.Bot.Models;
using Goblin.Domain.Entities;
using Goblin.Persistence;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenWeatherMap;
using Vk;
using Random = Goblin.Bot.Commands.Random;

namespace Goblin.WebUI.Extensions
{
    public static class ServiceProviderExtensions
    {
        public static void AddBotFeatures(this IServiceCollection services)
        {
            services.AddScoped<Handler>();
            services.AddScoped<CommandExecutor>();

            services.AddScoped<ICommand, AddRemind>();
            services.AddScoped<ICommand, Debug>();
            services.AddScoped<ICommand, Exams>();
            services.AddScoped<ICommand, FindTeacher>();
            services.AddScoped<ICommand, GetReminds>();
            services.AddScoped<ICommand, Keyboard>();
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
            services.AddScoped<ICommand, MuteErrors>();
        }

        public static void AddAdditions(this IServiceCollection services, IConfiguration config)
        {
            services.AddSingleton(x => new VkApi(config["Config:Vk_Token"]));
            services.AddSingleton(x => new WeatherService(config["Config:OWM_Token"]));
        }

        public static void AddAuth(this IServiceCollection services, IConfiguration config)
        {
            services.AddIdentity<SiteUser, IdentityRole>()
                    .AddRoles<IdentityRole>()
                    .AddDefaultUI(UIFramework.Bootstrap4)
                    .AddEntityFrameworkStores<ApplicationDbContext>();

            services.AddAuthentication()
                    .AddVkontakte(options =>
                    {
                        options.ApiVersion = "5.95";
                        options.ClientId = config["VkAuth:AppId"];
                        options.ClientSecret = config["VkAuth:AppSecret"];
                        options.Scope.Add("email");
                    });
        }

        public static void AddHttpsRedirect(this IServiceCollection services)
        {
            services.AddHttpsRedirection(options => { options.HttpsPort = 443; });
            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.KnownNetworks.Clear();
                options.KnownProxies.Clear();
                options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            });
        }
    }
}