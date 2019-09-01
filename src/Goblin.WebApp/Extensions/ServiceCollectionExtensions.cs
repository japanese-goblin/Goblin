using Goblin.Application;
using Goblin.Application.Abstractions;
using Goblin.Application.Commands.Keyboard;
using Goblin.Application.Commands.Merged;
using Goblin.Application.Commands.Text;
using Goblin.Application.Options;
using Goblin.DataAccess;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VkNet;
using VkNet.Abstractions;
using VkNet.Model;

namespace Goblin.WebApp.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddVkApi(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IVkApi, VkApi>(x =>
            {
                var token = configuration["Vk:AccessToken"];
                var api = new VkApi { RequestsPerSecond = 20 };
                api.Authorize(new ApiAuthParams
                {
                    AccessToken = token
                });
                return api;
            });
        }

        public static void AddDbContexts(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            services.AddDbContext<BotDbContext>(options =>
            {
                options.UseNpgsql(connectionString)
                       .UseLazyLoadingProxies();
            });
            services.AddDbContext<IdentityUsersDbContext>(options => options.UseNpgsql(connectionString));
        }

        public static void AddBotFeatures(this IServiceCollection services)
        {
            services.AddScoped<ITextCommand, DebugCommand>();
            services.AddScoped<ITextCommand, SetDataCommand>();
            services.AddScoped<ITextCommand, ChooseCommand>();
            services.AddScoped<ITextCommand, SendToAdminCommand>();
            services.AddScoped<ITextCommand, AddRemindCommand>();
            services.AddScoped<ITextCommand, FindTeacherCommand>();
            services.AddScoped<ITextCommand, RemoveKeyboardCommand>();
            services.AddScoped<ITextCommand, MuteCommand>();

            services.AddScoped<IKeyboardCommand, ScheduleKeyboardCommand>();
            services.AddScoped<IKeyboardCommand, WeatherDailyKeyboardCommand>();
            services.AddScoped<IKeyboardCommand, ScheduleCommand>();
            services.AddScoped<IKeyboardCommand, MailingKeyboardCommand>();
            services.AddScoped<IKeyboardCommand, MailingCommand>();
            services.AddScoped<IKeyboardCommand, TeacherScheduleCommand>();

            services.AddScoped<IKeyboardCommand, HelpCommand>();
            services.AddScoped<ITextCommand, HelpCommand>();
            services.AddScoped<IKeyboardCommand, ExamsCommand>();
            services.AddScoped<ITextCommand, ExamsCommand>();
            services.AddScoped<IKeyboardCommand, GetRemindsCommand>();
            services.AddScoped<ITextCommand, GetRemindsCommand>();
            services.AddScoped<IKeyboardCommand, WeatherNowCommand>();
            services.AddScoped<ITextCommand, WeatherNowCommand>();
            services.AddScoped<IKeyboardCommand, WeatherDailyCommand>();
            services.AddScoped<ITextCommand, WeatherDailyCommand>();
            services.AddScoped<IKeyboardCommand, StartCommand>();
            services.AddScoped<ITextCommand, StartCommand>();

            services.AddScoped<CommandsService>();
            services.AddScoped<CallbackHandler>();
        }

        public static void AddAuth(this IServiceCollection services, IConfiguration config)
        {
            services.AddIdentity<IdentityUser, IdentityRole>()
                    .AddDefaultUI(UIFramework.Bootstrap4)
                    .AddEntityFrameworkStores<IdentityUsersDbContext>();

            services.AddAuthentication()
                    .AddVkontakte(options =>
                    {
                        options.ApiVersion = "5.95";
                        options.ClientId = config["VkAuth:AppId"];
                        options.ClientSecret = config["VkAuth:SecretKey"];
                        options.Scope.Add("email");
                    });
        }

        public static void AddOptions(this IServiceCollection services, IConfiguration config)
        {
            services.Configure<VkOptions>(config.GetSection("Vk"));
            services.Configure<VkAuthOptions>(config.GetSection("VkAuth"));
            services.Configure<OpenWeatherMapOptions>(config.GetSection("OWM"));
        }
    }
}