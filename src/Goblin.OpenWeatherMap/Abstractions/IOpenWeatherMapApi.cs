using Goblin.OpenWeatherMap.Models.Daily;
using Goblin.OpenWeatherMap.Models.Responses;

namespace Goblin.OpenWeatherMap.Abstractions;

public interface IOpenWeatherMapApi
{
    /// <summary>
    /// Получить погоду в указанном городе на текущий момент
    /// </summary>
    /// <param name="city">Город, в котором нужно получить погоду</param>
    /// <returns>Данные о погоде в указанном городе</returns>
    /// <exception cref="System.Net.Http.HttpRequestException">Сервер вернул не успешный код</exception>
    public Task<CurrentWeatherResponse> GetCurrentWeather(string city);

    /// <summary>
    /// Получить погоду на день в указанном городе и на указанную дату
    /// </summary>
    /// <param name="city">Город</param>
    /// <param name="date">Дата для получения погоды</param>
    /// <returns>Данные о погоде на указанный день</returns>
    /// <exception cref="System.Net.Http.HttpRequestException">Сервер вернул не успешный код</exception>
    /// <exception cref="ArgumentException">Погода на указанную дату не найдена</exception>
    public Task<DailyWeatherListItem> GetDailyWeatherAt(string city, DateTime date);

    /// <summary>
    /// Проверка на существование города
    /// </summary>
    /// <param name="city">Город</param>
    /// <returns>Найден ли указанный город</returns>
    public Task<bool> IsCityExists(string city);
}