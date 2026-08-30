using Goblin.Narfu.Abstractions;
using Goblin.Narfu.Settings;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Goblin.Narfu.Groups;

internal sealed class NarfuGroupsRefreshService(
    INarfuGroupsParser parser,
    INarfuGroupsCache cache,
    IOptions<NarfuApiOptions> options,
    ILogger<NarfuGroupsRefreshService> logger) : BackgroundService
{
    private static readonly TimeSpan RetryInterval = TimeSpan.FromMinutes(1);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while(!stoppingToken.IsCancellationRequested)
        {
            var isSuccessful = await RefreshGroups(stoppingToken);
            var delay = isSuccessful ? options.Value.GroupsRefreshInterval : RetryInterval;
            await Task.Delay(delay, stoppingToken);
        }
    }

    private async Task<bool> RefreshGroups(CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("Начато обновление списка групп САФУ");
            var groups = await parser.ParseAsync(cancellationToken);
            await cache.ReplaceAsync(groups, cancellationToken);
            logger.LogInformation("В Redis сохранено {GroupsCount} групп САФУ", groups.Count);
            return true;
        }
        catch(OperationCanceledException) when(cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch(Exception exception)
        {
            logger.LogError(exception,
                            "Не удалось обновить список групп САФУ. Повтор через {RetryInterval}",
                            RetryInterval);
            return false;
        }
    }
}
