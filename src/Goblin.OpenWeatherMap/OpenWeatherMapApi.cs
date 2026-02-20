using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Goblin.OpenWeatherMap.Abstractions;
using Goblin.OpenWeatherMap.Models.Daily;
using Goblin.OpenWeatherMap.Models.Responses;
using Goblin.OpenWeatherMap.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Goblin.OpenWeatherMap;

public class OpenWeatherMapApi : IOpenWeatherMapApi
{
    private readonly ILogger _logger;
    private readonly HttpClient _client;
    private readonly OpenWeatherMapApiOptions _options;

    public OpenWeatherMapApi(IHttpClientFactory httpClientFactory, IOptions<OpenWeatherMapApiOptions> optionsAccessor, ILogger<OpenWeatherMapApi> logger)
    {
        _options = optionsAccessor.Value;
        _client = httpClientFactory.CreateClient(Defaults.HttpClientName);
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<CurrentWeatherResponse> GetCurrentWeather(string city)
    {
        _logger.LogDebug("Получение погоды на текущий момент в городе {City}", city);
        var response = await _client.GetFromJsonAsync<CurrentWeatherResponse>(GetWithDefaultQueryParams($"weather?q={city}"));
        _logger.LogDebug("Погода получена");

        return response;
    }

    /// <inheritdoc />
    public async Task<DailyWeatherListItem> GetDailyWeatherAt(string city, DateTime date)
    {
        _logger.LogDebug("Получение погоды на день в городе {City} на дату {WeatherDate:dd.MM.yyyy}", city, date);
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

        _logger.LogDebug("Погода получена");

        return weather;
    }

    /// <inheritdoc />
    public async Task<bool> IsCityExists(string city)
    {
        _logger.LogDebug("Проверка на существование города {City}", city);
        var response = await _client.SendAsync(new HttpRequestMessage(HttpMethod.Head, GetWithDefaultQueryParams($"weather/?q={city}")));
        return response.IsSuccessStatusCode;
    }

    private string GetWithDefaultQueryParams(string path)
    {
        var queries = $"units={_options.Units}&lang={_options.Language}&appid={_options.AccessToken}";
        return path.Contains('?') ? $"{path}&{queries}" : $"?{path}&{queries}";
    }
}