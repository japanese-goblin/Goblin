using System.Net.Http.Json;
using Goblin.OpenWeatherMap.Abstractions;
using Goblin.OpenWeatherMap.Models.Daily;
using Goblin.OpenWeatherMap.Models.Responses;
using Goblin.OpenWeatherMap.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Goblin.OpenWeatherMap;

public class OpenWeatherMapApi(
        IHttpClientFactory httpClientFactory,
        IOptions<OpenWeatherMapApiOptions> optionsAccessor,
        ILogger<OpenWeatherMapApi> logger)
        : IOpenWeatherMapApi
{
    private readonly HttpClient _client = httpClientFactory.CreateClient(Defaults.HttpClientName);
    private readonly OpenWeatherMapApiOptions _options = optionsAccessor.Value;

    /// <inheritdoc />
    public async Task<CurrentWeatherResponse> GetCurrentWeather(string city)
    {
        logger.LogDebug("Получение погоды на текущий момент в городе {City}", city);
        var response = await _client.GetFromJsonAsync<CurrentWeatherResponse>(GetWithDefaultQueryParams($"weather?q={city}"));
        logger.LogDebug("Погода получена");

        return response;
    }

    /// <inheritdoc />
    public async Task<DailyWeatherListItem> GetDailyWeatherAt(string city, DateTime date)
    {
        logger.LogDebug("Получение погоды на день в городе {City} на дату {WeatherDate:dd.MM.yyyy}", city, date);
        var response = await _client.GetFromJsonAsync<DailyWeatherResponse>(GetWithDefaultQueryParams($"forecast/daily?q={city}&cnt=4"));

        // разница между указанной и полученной меньше одного дня
        var weather = response.List.FirstOrDefault(x =>
        {
            var diff = (x.UnixTime.Date - date.Date).Days;
            return diff is >= 0 and <= 1;
        });

        if(weather is null)
        {
            throw new ArgumentException($"Погода на {date:dd.MM.yyyy} в городе {city} не найдена.");
        }

        logger.LogDebug("Погода получена");

        return weather;
    }

    /// <inheritdoc />
    public async Task<bool> IsCityExists(string city)
    {
        logger.LogDebug("Проверка на существование города {City}", city);
        var response = await _client.SendAsync(new HttpRequestMessage(HttpMethod.Head, GetWithDefaultQueryParams($"weather/?q={city}")));
        return response.IsSuccessStatusCode;
    }

    private string GetWithDefaultQueryParams(string path)
    {
        var queries = $"units={_options.Units}&lang={_options.Language}&appid={_options.AccessToken}";
        return path.Contains('?') ? $"{path}&{queries}" : $"?{path}&{queries}";
    }
}