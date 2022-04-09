using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Goblin.Application.Core;
using Goblin.Application.Core.Abstractions;
using Goblin.DataAccess;
using Goblin.Domain;
using Goblin.Domain.Abstractions;
using Goblin.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace Goblin.BackgroundJobs.Jobs;

public class WeatherTask
{
    private readonly BotDbContext _db;
    private readonly IEnumerable<ISender> _senders;
    private readonly ILogger _logger;
    private readonly IWeatherService _weatherService;

    public WeatherTask(IWeatherService weatherService, BotDbContext db, IEnumerable<ISender> senders)
    {
        _weatherService = weatherService;
        _db = db;
        _senders = senders;
        _logger = Log.ForContext<WeatherTask>();
    }

    public async Task Execute()
    {
        Func<string, IEnumerable<long>, ConsumerType, Task> send = async (text, userIds, consumer) =>
        {
            var sender = _senders.FirstOrDefault(x => x.ConsumerType == consumer);
            await sender.SendToMany(userIds, text);
        };

        await SendWeather<VkBotUser>(send);
        await SendWeather<TgBotUser>(send);
    }

    private async Task SendWeather<T>(Func<string, IEnumerable<long>, ConsumerType, Task> func) where T : BotUser
    {
        var grouped = _db.Set<T>()
                         .AsNoTracking()
                         .Where(x => x.HasWeatherSubscription)
                         .ToArray()
                         .GroupBy(x => x.WeatherCity);
        foreach(var group in grouped)
        {
            var result = await _weatherService.GetDailyWeather(group.Key, DateTime.Today);

            foreach(var chunk in group.Chunk(Defaults.ChunkLimit))
            {
                try
                {
                    var ids = chunk.Select(x => x.Id);
                    await func(result.Message, ids, chunk.FirstOrDefault().ConsumerType); //TODO:
                }
                catch(Exception ex)
                {
                    _logger.Error(ex, "Ошибка при отправке ежедневной погоды");
                }

                await Task.Delay(TimeSpan.FromSeconds(1.5));
            }
        }
    }
}