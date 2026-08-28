using Goblin.Application.Core;
using Goblin.Application.Vk.HostedServices;
using Goblin.Application.Vk.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using VkNet;
using VkNet.Abstractions;
using VkNet.Model;

namespace Goblin.Application.Vk;

public static class SetupExtensions
{
    private const string VkSettingsPath = "Vk";
    private const string VkLongPollingSettingsPath = "Vk:LongPolling";

    public static void AddVkLayer(this IServiceCollection services)
    {
        services.AddVkCore();

        services.AddOptions<VkCallbackOptions>()
                .BindConfiguration(VkSettingsPath)
                .ValidateDataAnnotations()
                .ValidateOnStart();

        services.AddSingleton<VkEventsDispatcher>();
        services.AddHostedService<VkChannelReaderHostedService>();
    }

    public static void AddVkLongPollingLayer(this IServiceCollection services)
    {
        services.AddVkCore();

        services.AddOptions<VkLongPollingOptions>()
                .BindConfiguration(VkLongPollingSettingsPath)
                .Validate(p => p.GroupId > 0, "Не задан ID сообщества VK")
                .Validate(p => p.WaitTimeout is > 0 and <= 90,
                    "Таймаут Long Poll должен быть в пределах от 1 до 90 секунд")
                .Validate(p => p.DelayBetweenUpdates >= TimeSpan.Zero,
                    "Задержка между запросами Long Poll не может быть отрицательной")
                .ValidateOnStart();

        services.AddHostedService<VkLongPollingHostedService>();
    }

    private static void AddVkCore(this IServiceCollection services)
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
