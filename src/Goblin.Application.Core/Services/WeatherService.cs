﻿using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Goblin.Application.Core.Abstractions;
using Goblin.Application.Core.Results.Failed;
using Goblin.Application.Core.Results.Success;
using Goblin.OpenWeatherMap.Abstractions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace Goblin.Application.Core.Services;

public class WeatherService : IWeatherService
{
    private const string DailyCacheKey = "Weather_Daily";
    private const string NowCacheKey = "Weather_Now";
    private const string NotFoundCacheKey = "Weather_NotFound";
    
    private static readonly TimeSpan CurrentWeatherExpireTime = TimeSpan.FromMinutes(10);
    private static readonly TimeSpan DailyWeatherExpireTime = TimeSpan.FromHours(3);
    private static readonly TimeSpan NotFoundExpireTime = TimeSpan.FromMinutes(15);
    
    private readonly IMemoryCache _cache;
    private readonly ILogger<WeatherService> _logger;
    private readonly IOpenWeatherMapApi _weatherMapApi;

    public WeatherService(IOpenWeatherMapApi weatherMapApi, IMemoryCache cache, ILogger<WeatherService> logger)
    {
        _weatherMapApi = weatherMapApi;
        _cache = cache;
        _logger = logger;
    }

    public async Task<IResult> GetCurrentWeather(string city)
    {
        try
        {
            var key = GetCurrentCacheKey(city);
            if(!_cache.TryGetValue<string>(key, out var result))
            {
                var weather = await _weatherMapApi.GetCurrentWeather(city);
                result = weather.ToString();
                _cache.Set(key, result, CurrentWeatherExpireTime);
            }

            return new SuccessfulResult
            {
                Message = result
            };
        }
        catch(HttpRequestException ex)
        {
            if(ex.StatusCode == HttpStatusCode.NotFound)
            {
                return new FailedResult($"Город \"{city}\" не найден");
            }

            _logger.LogError(ex, "Ошибка при получении погоды на текущий момент");
            return new FailedResult(DefaultErrors.WeatherSiteIsUnavailable);
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "Ошибка при получении погоды на текущий момент");
            return new FailedResult(DefaultErrors.WeatherUnexpectedError);
        }
    }

    public async Task<IResult> GetDailyWeather(string city, DateTime date)
    {
        try
        {
            var key = GetDailyCacheKey(city, date);
            if(!_cache.TryGetValue<string>(key, out var result))
            {
                var weather = await _weatherMapApi.GetDailyWeatherAt(city, date);

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

                _cache.Set(key, result, DailyWeatherExpireTime);
            }

            return new SuccessfulResult
            {
                Message = result
            };
        }
        catch(HttpRequestException ex)
        {
            if(ex.StatusCode == HttpStatusCode.NotFound)
            {
                var result = SetNotFoundCacheValue(city);
                return new FailedResult(result);
            }

            _logger.LogError(ex, "Ошибка при получении погоды на текущий момент");
            return new FailedResult(DefaultErrors.WeatherSiteIsUnavailable);
        }
        catch(ArgumentException ex)
        {
            return new FailedResult(ex.Message);
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "Ошибка при получении погоды на день");
            return new FailedResult(DefaultErrors.WeatherUnexpectedError);
        }
    }

    private string SetNotFoundCacheValue(string city)
    {
        var key = GetNotFoundCacheKey(city);
        var result = $"Город \"{city}\" не найден";

        _cache.Set(key, result, NotFoundExpireTime);
        return result;
    }

    private static string GetCurrentCacheKey(string city)
    {
        return $"{NowCacheKey}_{city}";
    }

    private static string GetDailyCacheKey(string city, DateTime date)
    {
        return $"{DailyCacheKey}_{city}_{date:dd.MM.yyyy}";
    }

    private static string GetNotFoundCacheKey(string city)
    {
        return $"{NotFoundCacheKey}_{city}";
    }
}