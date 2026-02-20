using System;
using Goblin.OpenWeatherMap.Abstractions;
using Goblin.OpenWeatherMap.Settings;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Goblin.OpenWeatherMap;

public static class SetupExtensions
{
    private const string OwmSettingsPath = "Clients:OpenWeatherMap";

    public static IServiceCollection AddOpenWeatherMapApi(this IServiceCollection services)
    {
        services.AddSingleton<IOpenWeatherMapApi, OpenWeatherMapApi>();

        services.AddSingleton<IValidateOptions<OpenWeatherMapApiOptions>, OpenWeatherMapApiOptionsValidate>();
        services.AddOptions<OpenWeatherMapApiOptions>()
                .BindConfiguration(OwmSettingsPath)
                .ValidateOnStart();

        services.AddHttpClient(Defaults.HttpClientName, (sp, client) =>
        {
            var options = sp.GetRequiredService<IOptions<OpenWeatherMapApiOptions>>().Value;

            client.BaseAddress = new Uri(options.HostUrl);
            client.Timeout = options.Timeout;
        });

        return services;
    }
}