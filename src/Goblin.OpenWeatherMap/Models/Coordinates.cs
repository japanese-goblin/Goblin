using Newtonsoft.Json;

namespace Goblin.OpenWeatherMap.Models;

public class Coordinates
{
    /// <summary>
    /// Долгота
    /// </summary>
    [JsonProperty("lon")]
    public double Longitude { get; set; }

    /// <summary>
    /// Широта
    /// </summary>
    [JsonProperty("lat")]
    public double Latitude { get; set; }
}