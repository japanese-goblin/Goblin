using System.Text.Json.Serialization;

namespace Goblin.OpenWeatherMap.Models.Daily;

public class FeelsLike
{
    /// <summary>
    /// Температура утром, которая учитывает восприятие погоды человеком
    /// </summary>
    [JsonPropertyName("morning")]
    public double Morning { get; set; }

    /// <summary>
    /// Температура днём, которая учитывает восприятие погоды человеком
    /// </summary>
    [JsonPropertyName("day")]
    public double Day { get; set; }

    /// <summary>
    /// Температура вечером, которая учитывает восприятие погоды человеком
    /// </summary>
    [JsonPropertyName("eve")]
    public double Evening { get; set; }

    /// <summary>
    /// Температура ночью, которая учитывает восприятие погоды человеком
    /// </summary>
    [JsonPropertyName("night")]
    public double Night { get; set; }
}