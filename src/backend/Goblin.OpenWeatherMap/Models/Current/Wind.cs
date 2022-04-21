using System.Text.Json.Serialization;

namespace Goblin.OpenWeatherMap.Models.Current;

public class Wind
{
    /// <summary>
    /// Направление ветра (в градусах)
    /// </summary>
    [JsonPropertyName("deg")]
    public double Degrees { get; set; }

    /// <summary>
    /// Порыв ветра
    /// </summary>
    [JsonPropertyName("gust")]
    public double? Gust { get; set; }

    /// <summary>
    /// Скорость ветра
    /// </summary>
    [JsonPropertyName("speed")]
    public double Speed { get; set; }
}