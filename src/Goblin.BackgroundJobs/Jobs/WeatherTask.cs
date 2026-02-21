using Goblin.Application.Core;
using Goblin.Application.Core.Abstractions;
using Goblin.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Goblin.BackgroundJobs.Jobs;

public class WeatherTask
{
    private readonly BotDbContext _db;
    private readonly IEnumerable<ISender> _senders;
    private readonly ILogger<WeatherTask> _logger;
    private readonly IWeatherService _weatherService;

    public WeatherTask(IWeatherService weatherService, BotDbContext db,
                       IEnumerable<ISender> senders, ILogger<WeatherTask> logger)
    {
        _weatherService = weatherService;
        _db = db;
        _senders = senders;
        _logger = logger;
    }

    public async Task Execute()
    {
        var consumersGroup = _db.BotUsers.AsNoTracking().Where(x => x.HasWeatherSubscription)
                                .ToArray()
                                .GroupBy(x => x.ConsumerType);
        foreach(var consumerGroup in consumersGroup)
        {
            var sender = _senders.First(x => x.ConsumerType == consumerGroup.Key);
            var groupedByCity = consumerGroup.GroupBy(x => x.WeatherCity);
            foreach(var group in groupedByCity)
            {
                if(string.IsNullOrWhiteSpace(group.Key))
                {
                    continue;
                }

                var result = await _weatherService.GetDailyWeather(group.Key, DateTime.Today);

                foreach(var chunk in group.Chunk(Defaults.ChunkLimit))
                {
                    try
                    {
                        var ids = chunk.Select(x => x.Id).ToList();
                        await sender.SendToMany(ids, result.Message);
                    }
                    catch(Exception ex)
                    {
                        _logger.LogError(ex, "Ошибка при отправке ежедневной погоды");
                    }

                    await Task.Delay(TimeSpan.FromSeconds(1.5));
                }
            }
        }
    }
}