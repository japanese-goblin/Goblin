using Goblin.Application.Core;
using Goblin.Application.Vk.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using VkNet;
using VkNet.Abstractions;
using VkNet.Model;

namespace Goblin.Application.Vk;

public static class DependencyInjection
{
    private const string VkSettingsPath = "Vk";

    public static void AddVkLayer(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<VkOptions>()
                .BindConfiguration(VkSettingsPath)
                .ValidateDataAnnotations()
                .ValidateOnStart();

        services.AddSingleton<IVkApi, VkApi>(sp =>
        {
            var options = sp.GetRequiredService<IOptions<VkOptions>>();
            var api = new VkApi { RequestsPerSecond = 20 };
            api.Authorize(new ApiAuthParams
            {
                AccessToken = options.Value.AccessToken
            });
            return api;
        });

        services.AddScoped<VkCallbackHandler>();

        services.AddScoped<ISender, VkSender>();
    }
}