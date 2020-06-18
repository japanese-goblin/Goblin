using AutoMapper;
using Goblin.Application.Core.Options;
using Hangfire;
using Hangfire.MemoryStorage;
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
            AddVkNet();
            AddAutoMapper();
            AddVkOptions();

            services.AddHangfire(config => { config.UseMemoryStorage(); });
            services.AddScoped<CallbackHandler>();

            void AddVkNet()
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

            void AddAutoMapper()
            {
                var mappingConfig = new MapperConfiguration(mc => { mc.AddProfile(new MappingProfile()); });

                var mapper = mappingConfig.CreateMapper();
                services.AddSingleton(mapper);
            }

            void AddVkOptions()
            {
                services.Configure<VkOptions>(configuration.GetSection("Vk"));
                services.Configure<VkAuthOptions>(configuration.GetSection("VkAuth"));
            }
        }
    }
}