using System.Text.Json.Serialization;

namespace Goblin.OpenWeatherMap.Models;

public class Coordinates
{
    /// <summary>
    /// Долгота
    /// </summary>
    [JsonPropertyName("lon")]
    public double Longitude { get; set; }

    /// <summary>
    /// Широта
    /// </summary>
    [JsonPropertyName("lat")]
    public double Latitude { get; set; }
}