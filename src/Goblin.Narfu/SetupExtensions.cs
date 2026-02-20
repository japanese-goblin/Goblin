using Goblin.Narfu.Abstractions;
using Goblin.Narfu.Settings;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Goblin.Narfu;

public static class SetupExtensions
{
    private const string NarfuSettingsPath = "Clients:Narfu";

    public static IServiceCollection AddNarfuApi(this IServiceCollection services)
    {
        services.AddSingleton<INarfuApi, NarfuApi>();

        services.AddSingleton<IValidateOptions<NarfuApiOptions>, NarfuOptionsValidate>();
        services.AddOptions<NarfuApiOptions>()
                .BindConfiguration(NarfuSettingsPath)
                .ValidateOnStart();

        services.AddHttpClient(Defaults.HttpClientName, (sp, client) =>
        {
            var optionsAccessor = sp.GetRequiredService<IOptions<NarfuApiOptions>>();

            client.BaseAddress = new Uri(optionsAccessor.Value.HostUrl);
            client.Timeout = optionsAccessor.Value.Timeout;

            client.DefaultRequestHeaders.UserAgent.Clear();
            client.DefaultRequestHeaders.Add("UserAgent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:99.0) Gecko/20100101 Firefox/99.0");
        });

        return services;
    }
}