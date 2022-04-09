using System;
using System.Linq;
using System.Threading.Tasks;
using Flurl.Http;
using Goblin.OpenWeatherMap.Abstractions;
using Goblin.OpenWeatherMap.Models.Daily;
using Goblin.OpenWeatherMap.Models.Responses;
using Serilog;

namespace Goblin.OpenWeatherMap;

public class OpenWeatherMapApi : IOpenWeatherMapApi
{
    private readonly ILogger _logger;
    private readonly string _token;

    public OpenWeatherMapApi(string token)
    {
        _logger = Log.ForContext<OpenWeatherMapApi>();

        _logger.Debug("Инициализация {0}", nameof(OpenWeatherMapApi));
        if(string.IsNullOrWhiteSpace(token))
        {
            _logger.Fatal("Токен пуст");
            throw new ArgumentException("Токен пуст");
        }

        _token = token;
    }

    /// <inheritdoc />
    public async Task<CurrentWeatherResponse> GetCurrentWeather(string city)
    {
        _logger.Debug("Получение погоды на текущий момент в городе {0}", city);
        var response = await RequestBuilder.Create(_token)
                                           .AppendPathSegment("weather")
                                           .SetQueryParam("q", city)
                                           .GetJsonAsync<CurrentWeatherResponse>();
        _logger.Debug("Погода получена");

        return response;
    }

    /// <inheritdoc />
    public async Task<DailyWeatherListItem> GetDailyWeatherAt(string city, DateTime date)
    {
        _logger.Debug("Получение погоды на день в городе {0} на дату {1:dd.MM.yyyy}", city, date);

        var response = await RequestBuilder.Create(_token)
                                           .AppendPathSegments("forecast", "daily")
                                           .SetQueryParam("q", city)
                                           .SetQueryParam("cnt", 4)
                                           .GetJsonAsync<DailyWeatherResponse>();

        // разница между указанной и полученной меньше одного дня
        var weather = response.List.FirstOrDefault(x =>
        {
            var diff = (x.UnixTime.Date - date.Date).Days;
            return diff >= 0 && diff <= 1;
        });
            
        if(weather is null)
        {
            var msg = $"Погода на {date:dd.MM.yyyy} в городе {city} не найдена.";
            _logger.Warning(msg);
            throw new ArgumentException(msg);
        }

        _logger.Debug("Погода получена");

        return weather;
    }

    /// <inheritdoc />
    public async Task<bool> IsCityExists(string city)
    {
        _logger.Debug("Проверка на существование города {0}", city);
        var response = await RequestBuilder.Create(_token)
                                           .AllowAnyHttpStatus()
                                           .AppendPathSegment("weather")
                                           .SetQueryParam("q", city)
                                           .HeadAsync();

        return response.ResponseMessage.IsSuccessStatusCode;
    }
}