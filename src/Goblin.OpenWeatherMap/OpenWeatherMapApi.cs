using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Goblin.OpenWeatherMap.Abstractions;
using Goblin.OpenWeatherMap.Models.Daily;
using Goblin.OpenWeatherMap.Models.Responses;
using Serilog;

namespace Goblin.OpenWeatherMap;

public class OpenWeatherMapApi : IOpenWeatherMapApi
{
    private readonly ILogger _logger;
    private readonly string _token;
    private readonly HttpClient _client;

    public OpenWeatherMapApi(string token, IHttpClientFactory factory)
    {
        _logger = Log.ForContext<OpenWeatherMapApi>();
        _logger.Debug("Инициализация {Class}", nameof(OpenWeatherMapApi));
        
        if(string.IsNullOrWhiteSpace(token))
        {
            throw new ArgumentException("Токен пуст");
        }
        _token = token;

        _client = factory.CreateClient("open-weather-map-api");
        _client.Timeout = TimeSpan.FromSeconds(5);
        _client.BaseAddress = new Uri("https://api.openweathermap.org/data/2.5/");
    }

    /// <inheritdoc />
    public async Task<CurrentWeatherResponse> GetCurrentWeather(string city)
    {
        _logger.Debug("Получение погоды на текущий момент в городе {City}", city);
        var response = await _client.GetFromJsonAsync<CurrentWeatherResponse>(GetWithDefaultQueryParams($"weather?q={city}"));
        _logger.Debug("Погода получена");

        return response;
    }

    /// <inheritdoc />
    public async Task<DailyWeatherListItem> GetDailyWeatherAt(string city, DateTime date)
    {
        _logger.Debug("Получение погоды на день в городе {City} на дату {WeatherDate:dd.MM.yyyy}", city, date);
        var response = await _client.GetFromJsonAsync<DailyWeatherResponse>(GetWithDefaultQueryParams($"forecast/daily?q={city}&cnt=4"));

        // разница между указанной и полученной меньше одного дня
        var weather = response.List.FirstOrDefault(x =>
        {
            var diff = (x.UnixTime.Date - date.Date).Days;
            return diff >= 0 && diff <= 1;
        });
            
        if(weather is null)
        {
            throw new ArgumentException($"Погода на {date:dd.MM.yyyy} в городе {city} не найдена.");
        }

        _logger.Debug("Погода получена");

        return weather;
    }

    /// <inheritdoc />
    public async Task<bool> IsCityExists(string city)
    {
        _logger.Debug("Проверка на существование города {City}", city);
        var response = await _client.SendAsync(new HttpRequestMessage(HttpMethod.Head, GetWithDefaultQueryParams($"weather/?q={city}")));
        return response.IsSuccessStatusCode;
    }

    private string GetWithDefaultQueryParams(string path)
    {
        const string language = "ru";
        const string units = "metric";

        var queries = $"units={units}&lang={language}&appid={_token}";
        return path.Contains('?') ? $"{path}&{queries}" : $"?{path}&{queries}";
    }
}