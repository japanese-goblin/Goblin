using Goblin.Application.Core;
using Goblin.Application.Telegram.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;

namespace Goblin.Application.Telegram;

public static class DependencyInjection
{
    public static void AddTelegramLayer(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<TelegramOptions>(configuration.GetSection("Telegram"));

        services.AddSingleton(new TelegramBotClient(configuration["Telegram:AccessToken"]));
        services.AddScoped<TelegramCallbackHandler>();

        services.AddScoped<ISender, TelegramSender>();
    }
}