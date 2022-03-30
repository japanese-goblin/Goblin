using Goblin.OpenWeatherMap.Models.Daily;
using Newtonsoft.Json;

namespace Goblin.OpenWeatherMap.Models.Responses;

public class DailyWeatherResponse
{
    /// <summary>
    /// Город
    /// </summary>
    [JsonProperty("city")]
    public City City { get; set; }

    /// <summary>
    /// Код ответа
    /// </summary>
    [JsonProperty("cod")]
    public int Code { get; set; }

    /// <summary>
    /// Сообщение (?)
    /// </summary>
    [JsonProperty("message")]
    public double Message { get; set; }

    /// <summary>
    /// Количество объектов в <see cref="List"/>
    /// </summary>
    [JsonProperty("cnt")]
    public long Count { get; set; }

    /// <summary>
    /// Данные о погоде по дням
    /// </summary>
    [JsonProperty("list")]
    public DailyWeatherListItem[] List { get; set; }
}