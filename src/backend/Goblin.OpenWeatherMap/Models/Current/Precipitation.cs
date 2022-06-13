using System.Text.Json.Serialization;

namespace Goblin.OpenWeatherMap.Models.Current;

public class Precipitation
{
    /// <summary>
    /// Количество осадков за последний час
    /// </summary>
    [JsonPropertyName("1h")]
    public double? ForLastOneHour { get; set; }

    /// <summary>
    /// Количество осадков за последние три часа
    /// </summary>
    [JsonPropertyName("3h")]
    public double? ForLastThreeHours { get; set; }
}