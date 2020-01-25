using Goblin.Application.Abstractions;
using Goblin.Application.Commands.Keyboard;
using Goblin.Application.Commands.Merged;
using Goblin.Application.Commands.Text;
using Goblin.Application.Options;
using Goblin.Narfu;
using Goblin.OpenWeatherMap;
using Hangfire;
using Hangfire.MemoryStorage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VkNet;
using VkNet.Abstractions;
using VkNet.Model;

namespace Goblin.Application
{
    public static class DependencyInjection
    {
        public static void AddApplication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddVkApi(configuration);
            services.AddBotFeatures();
            services.AddOptions(configuration);

            services.AddHangfire(config => { config.UseMemoryStorage(); });

            services.AddSingleton(x =>
            {
                var api = new OpenWeatherMapApi(configuration["OWM:AccessToken"]);
                return api;
            });
            services.AddSingleton<NarfuApi>();
        }

        private static void AddVkApi(this IServiceCollection services, IConfiguration configuration)
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

        private static void AddBotFeatures(this IServiceCollection services)
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
            services.AddScoped<IKeyboardCommand, GetRemindsCommand>();
            services.AddScoped<IKeyboardCommand, ExamsCommand>();

            services.AddScoped<IKeyboardCommand, HelpCommand>();
            services.AddScoped<ITextCommand, HelpCommand>();
            services.AddScoped<IKeyboardCommand, WeatherNowCommand>();
            services.AddScoped<ITextCommand, WeatherNowCommand>();
            services.AddScoped<IKeyboardCommand, WeatherDailyCommand>();
            services.AddScoped<ITextCommand, WeatherDailyCommand>();
            services.AddScoped<IKeyboardCommand, StartCommand>();
            services.AddScoped<ITextCommand, StartCommand>();

            services.AddScoped<CommandsService>();
            services.AddScoped<CallbackHandler>();
        }

        private static void AddOptions(this IServiceCollection services, IConfiguration config)
        {
            services.Configure<VkOptions>(config.GetSection("Vk"));
            services.Configure<VkAuthOptions>(config.GetSection("VkAuth"));
            services.Configure<OpenWeatherMapOptions>(config.GetSection("OWM"));
        }
    }
}