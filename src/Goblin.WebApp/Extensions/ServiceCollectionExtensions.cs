using Goblin.Application;
using Goblin.Application.Abstractions;
using Goblin.Application.Commands;
using Goblin.Application.KeyboardCommands;
using Goblin.DataAccess;
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
                var api = new VkApi();
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

            services.AddDbContext<BotDbContext>(options => options.UseSqlServer(connectionString));
            services.AddDbContext<IdentityUsersDbContext>(options => options.UseSqlServer(connectionString));
        }

        public static void AddBotFeatures(this IServiceCollection services)
        {
            services.AddScoped<ITextCommand, DebugCommand>();

            services.AddScoped<IKeyboardCommand, StartCommand>();
            
            services.AddScoped<CommandsService>();
            services.AddScoped<CallbackHandler>();
        }
    }
}