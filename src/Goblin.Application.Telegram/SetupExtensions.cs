using Goblin.Application.Core;
using Goblin.Application.Telegram.HostedServices;
using Goblin.Application.Telegram.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Telegram.Bot;

namespace Goblin.Application.Telegram;

public static class SetupExtensions
{
    private const string TelegramSettingsPath = "Telegram";

    public static void AddTelegramLayer(this IServiceCollection services)
    {
        services.AddOptions<TelegramOptions>()
                .BindConfiguration(TelegramSettingsPath)
                .ValidateDataAnnotations()
                .ValidateOnStart();

        services.AddSingleton(sp =>
        {
            var optionsAccessor = sp.GetRequiredService<IOptions<TelegramOptions>>();
            return new TelegramBotClient(optionsAccessor.Value.AccessToken);
        });

        services.AddScoped<TelegramCallbackHandler>();
        services.AddScoped<ISender, TelegramSender>();

        services.AddSingleton<TelegramEventsDispatcher>();
        services.AddHostedService<TelegramChannelReaderHostedService>();
    }
}