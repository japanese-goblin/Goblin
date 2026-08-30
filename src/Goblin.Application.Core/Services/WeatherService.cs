using System.Net;
using Goblin.OpenWeatherMap.Abstractions;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace Goblin.Application.Core.Services;

internal class WeatherService(IOpenWeatherMapApi weatherMapApi,
                              IDistributedCache distributedCache,
                              ILogger<WeatherService> logger)
        : IWeatherService
{
    private const string CachePrefix = "weather_data";

    private static readonly TimeSpan CurrentWeatherExpireTime = TimeSpan.FromMinutes(10);
    private static readonly TimeSpan DailyWeatherExpireTime = TimeSpan.FromHours(3);
    private static readonly TimeSpan NotFoundExpireTime = TimeSpan.FromMinutes(15);

    public async Task<CommandExecutionResult> GetCurrentWeather(string city, CancellationToken ct = default)
    {
        try
        {
            var key = GetCurrentCacheKey(city);
            var result = await distributedCache.GetStringAsync(key, ct);
            if(result is null)
            {
                var weather = await weatherMapApi.GetCurrentWeather(city);
                result = weather.ToString();
                await distributedCache.SetStringAsync(key, result, new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = CurrentWeatherExpireTime
                }, ct);
            }

            return CommandExecutionResult.Success(result);
        }
        catch(HttpRequestException ex) when(ex.StatusCode == HttpStatusCode.NotFound)
        {
            var result = await SetNotFoundCacheValue(city, ct);
            return CommandExecutionResult.Failed(result);
        }
        catch(Exception ex)
        {
            logger.LogError(ex, "Ошибка при получении погоды на текущий момент");
            return CommandExecutionResult.Failed(DefaultErrors.WeatherUnexpectedError);
        }
    }

    public async Task<CommandExecutionResult> GetDailyWeather(string city, DateTime date, CancellationToken ct = default)
    {
        try
        {
            var key = GetDailyCacheKey(city, date);
            var result = await distributedCache.GetStringAsync(key, ct);
            if(result is null)
            {
                var weather = await weatherMapApi.GetDailyWeatherAt(city, date);

                string formattedDate;
                if(date.Date == DateTime.Today)
                {
                    formattedDate = $"сегодня ({date:dd.MM, dddd})";
                }
                else if(date.Date == DateTime.Today.AddDays(1))
                {
                    formattedDate = $"завтра ({date:dd.MM, dddd})";
                }
                else
                {
                    formattedDate = $"({date:dd.MM (dddd)})";
                }

                result = $"Погода в городе {city} на {formattedDate}:\n{weather}";

                await distributedCache.SetStringAsync(key, result, new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = DailyWeatherExpireTime
                }, ct);
            }

            return CommandExecutionResult.Success(result);
        }
        catch(HttpRequestException ex) when(ex.StatusCode == HttpStatusCode.NotFound)
        {
            var result = await SetNotFoundCacheValue(city, ct);
            return CommandExecutionResult.Failed(result);
        }
        catch(ArgumentException ex)
        {
            return CommandExecutionResult.Failed(ex.Message);
        }
        catch(Exception ex)
        {
            logger.LogError(ex, "Ошибка при получении погоды на день");
            return CommandExecutionResult.Failed(DefaultErrors.WeatherUnexpectedError);
        }
    }

    private async Task<string> SetNotFoundCacheValue(string city, CancellationToken ct)
    {
        var key = GetNotFoundCacheKey(city);
        var result = $"Город \"{city}\" не найден";

        await distributedCache.SetStringAsync(key, result, new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = NotFoundExpireTime
        }, ct);
        return result;
    }

    private static string GetCurrentCacheKey(string city)
    {
        return $"{CachePrefix}:{city}:current";
    }

    private static string GetDailyCacheKey(string city, DateTime date)
    {
        return $"{CachePrefix}:{city}:forecast:{date:dd_MM_yyyy}";
    }

    private static string GetNotFoundCacheKey(string city)
    {
        return $"{CachePrefix}:{city}:not_found";
    }
}