using System.Text.Json.Serialization;
using Goblin.OpenWeatherMap.Models.Daily;

namespace Goblin.OpenWeatherMap.Models.Responses;

public class DailyWeatherResponse
{
    /// <summary>
    /// Город
    /// </summary>
    [JsonPropertyName("city")]
    public City City { get; set; }

    /// <summary>
    /// Код ответа
    /// </summary>
    [JsonPropertyName("cod")]
    public int Code { get; set; }

    /// <summary>
    /// Сообщение (?)
    /// </summary>
    [JsonPropertyName("message")]
    public double Message { get; set; }

    /// <summary>
    /// Количество объектов в <see cref="List"/>
    /// </summary>
    [JsonPropertyName("cnt")]
    public long Count { get; set; }

    /// <summary>
    /// Данные о погоде по дням
    /// </summary>
    [JsonPropertyName("list")]
    public DailyWeatherListItem[] List { get; set; }
}