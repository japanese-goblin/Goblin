using System.Text.Json.Serialization;

namespace Goblin.OpenWeatherMap.Models.Current;

public class Clouds
{
    /// <summary>
    /// Процент облачности
    /// </summary>
    [JsonPropertyName("all")]
    public int Cloudiness { get; set; }
}