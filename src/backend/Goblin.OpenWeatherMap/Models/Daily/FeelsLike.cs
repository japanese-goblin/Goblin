using Newtonsoft.Json;

namespace Goblin.OpenWeatherMap.Models.Daily;

public class FeelsLike
{
    /// <summary>
    /// Температура утром, которая учитывает восприятие погоды человеком
    /// </summary>
    [JsonProperty("morning")]
    public double Morning { get; set; }

    /// <summary>
    /// Температура днём, которая учитывает восприятие погоды человеком
    /// </summary>
    [JsonProperty("day")]
    public double Day { get; set; }

    /// <summary>
    /// Температура вечером, которая учитывает восприятие погоды человеком
    /// </summary>
    [JsonProperty("eve")]
    public double Evening { get; set; }

    /// <summary>
    /// Температура ночью, которая учитывает восприятие погоды человеком
    /// </summary>
    [JsonProperty("night")]
    public double Night { get; set; }
}