using System.Text.Json.Serialization;

namespace Goblin.OpenWeatherMap.Models.Daily;

public class Temperature
{
    /// <summary>
    /// Температура утром
    /// </summary>
    [JsonPropertyName("morn")]
    public double Morning { get; set; }

    /// <summary>
    /// Температура днём
    /// </summary>
    [JsonPropertyName("day")]
    public double Day { get; set; }

    /// <summary>
    /// Температура вечером
    /// </summary>
    [JsonPropertyName("eve")]
    public double Evening { get; set; }

    /// <summary>
    /// Температура ночью
    /// </summary>
    [JsonPropertyName("night")]
    public double Night { get; set; }

    /// <summary>
    /// Минимальная температура за день
    /// </summary>
    [JsonPropertyName("min")]
    public double Min { get; set; }

    /// <summary>
    /// Максимальная температура за день
    /// </summary>
    [JsonPropertyName("max")]
    public double Max { get; set; }
}