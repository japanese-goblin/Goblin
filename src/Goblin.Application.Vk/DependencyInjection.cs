using Goblin.Application.Vk.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VkNet;
using VkNet.Abstractions;
using VkNet.Model;

namespace Goblin.Application.Vk
{
    public static class DependencyInjection
    {
        public static void AddVkLayer(this IServiceCollection services, IConfiguration configuration)
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

            services.Configure<VkOptions>(configuration.GetSection("Vk"));
            services.Configure<VkAuthOptions>(configuration.GetSection("VkAuth"));

            services.AddScoped<VkCallbackHandler>();
        }
    }
}