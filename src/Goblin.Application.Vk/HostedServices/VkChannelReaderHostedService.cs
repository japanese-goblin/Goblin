using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Goblin.Application.Vk.HostedServices;

internal class VkChannelReaderHostedService(VkEventsDispatcher dispatcher, IServiceProvider serviceProvider, ILogger<VkChannelReaderHostedService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while(!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await foreach(var @event in dispatcher.ReadAllAsync(stoppingToken))
                {
                    await using var scope = serviceProvider.CreateAsyncScope();
                    var callbackHandler = scope.ServiceProvider.GetRequiredService<VkCallbackHandler>();
                    await callbackHandler.Handle(@event);
                }
            }
            catch(Exception e) when(!stoppingToken.IsCancellationRequested)
            {
                logger.LogError(e, "Не удалось обработать событие из очереди");
            }
        }
    }
}