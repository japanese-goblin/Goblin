using Goblin.Application.Core;
using Goblin.Application.Core.Abstractions;
using Goblin.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Quartz;

namespace Goblin.BackgroundJobs.Jobs;

public class SendWeatherForecastJob(
    IWeatherService weatherService,
    BotDbContext db,
    IEnumerable<ISender> senders,
    ILogger<SendWeatherForecastJob> logger)
    : IJob
{
    public static readonly JobKey JobKey = new("V1", "send-weather-forecast");

    public async Task Execute(IJobExecutionContext context)
    {
        var consumersGroup = db.BotUsers
            .AsNoTracking()
            .Where(p => p.HasWeatherSubscription)
            .ToArray()
            .GroupBy(p => p.ConsumerType);
        foreach (var consumerGroup in consumersGroup)
        {
            var sender = senders.First(p => p.ConsumerType == consumerGroup.Key);
            var groupedByCity = consumerGroup.GroupBy(p => p.WeatherCity);
            foreach (var group in groupedByCity)
            {
                if (string.IsNullOrWhiteSpace(group.Key))
                {
                    continue;
                }

                var result = await weatherService.GetDailyWeather(group.Key, DateTime.Today);

                foreach (var chunk in group.Chunk(Defaults.ChunkLimit))
                {
                    try
                    {
                        var ids = chunk.Select(p => p.ConsumerId).ToList();
                        await sender.SendToMany(ids, result.Message);
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "Ошибка при отправке ежедневной погоды");
                    }

                    await Task.Delay(Defaults.DelayBetweenSends);
                }
            }
        }
    }
}