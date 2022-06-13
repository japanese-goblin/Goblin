using System.Text.Json.Serialization;

namespace Goblin.OpenWeatherMap.Models.Daily;

public class City
{
    /// <summary>
    /// Идентификатор города
    /// </summary>
    [JsonPropertyName("id")]
    public long Id { get; set; }

    /// <summary>
    /// Название города
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; }

    /// <summary>
    /// Координаты города
    /// </summary>
    [JsonPropertyName("coord")]
    public Coordinates Coordinates { get; set; }

    /// <summary>
    /// Страна, в котором расположен город
    /// </summary>
    [JsonPropertyName("country")]
    public string Country { get; set; }

    /// <summary>
    /// Население города
    /// </summary>
    [JsonPropertyName("population")]
    public long Population { get; set; }
}