using Newtonsoft.Json;

namespace Goblin.OpenWeatherMap.Models.Daily;

public class Temperature
{
    /// <summary>
    /// Температура утром
    /// </summary>
    [JsonProperty("morn")]
    public double Morning { get; set; }

    /// <summary>
    /// Температура днём
    /// </summary>
    [JsonProperty("day")]
    public double Day { get; set; }

    /// <summary>
    /// Температура вечером
    /// </summary>
    [JsonProperty("eve")]
    public double Evening { get; set; }

    /// <summary>
    /// Температура ночью
    /// </summary>
    [JsonProperty("night")]
    public double Night { get; set; }

    /// <summary>
    /// Минимальная температура за день
    /// </summary>
    [JsonProperty("min")]
    public double Min { get; set; }

    /// <summary>
    /// Максимальная температура за день
    /// </summary>
    [JsonProperty("max")]
    public double Max { get; set; }
}