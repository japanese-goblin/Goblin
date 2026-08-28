using Goblin.Application.Vk.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using VkNet.Abstractions;
using VkNet.Utils.BotsLongPoll;

namespace Goblin.Application.Vk.HostedServices;

internal sealed class VkLongPollingHostedService(
    IVkApi vkApi,
    IServiceScopeFactory scopeFactory,
    IOptions<VkLongPollingOptions> optionsAccessor,
    ILogger<VkLongPollingHostedService> logger) : BackgroundService
{
    private readonly VkLongPollingOptions _options = optionsAccessor.Value;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var handlerOptions = new BotsLongPollUpdatesHandlerParams(vkApi, _options.GroupId)
        {
            DelayBetweenUpdates = _options.DelayBetweenUpdates,
            WaitTimeout = _options.WaitTimeout,
            OnUpdates = updates => ProcessUpdatesAsync(updates, stoppingToken).GetAwaiter().GetResult(),
            OnException = exception => logger.LogError(exception, "Критическая ошибка VK Long Poll"),
            OnWarn = exception => logger.LogWarning(exception, "Временная ошибка VK Long Poll")
        };

        logger.LogInformation("Запуск VK Long Poll для сообщества {GroupId}", _options.GroupId);

        var handler = new BotsLongPollUpdatesHandler(handlerOptions);
        await handler.RunAsync(stoppingToken);
    }

    private async Task ProcessUpdatesAsync(
        BotsLongPollOnUpdatesEvent updatesEvent,
        CancellationToken cancellationToken)
    {
        foreach (var updateEvent in updatesEvent.Updates)
        {
            if (updateEvent.Exception is not null)
            {
                logger.LogError(updateEvent.Exception, "Не удалось десериализовать событие VK Long Poll");
                continue;
            }

            if (updateEvent.Update is null)
            {
                logger.LogWarning("VK Long Poll вернул пустое событие");
                continue;
            }

            await using var scope = scopeFactory.CreateAsyncScope();
            var callbackHandler = scope.ServiceProvider.GetRequiredService<VkCallbackHandler>();
            await callbackHandler.Handle(updateEvent.Update, cancellationToken);
        }
    }
}
