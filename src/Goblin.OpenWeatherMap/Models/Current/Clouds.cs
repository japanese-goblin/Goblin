using Newtonsoft.Json;

namespace Goblin.OpenWeatherMap.Models.Current;

public class Clouds
{
    /// <summary>
    /// Процент облачности
    /// </summary>
    [JsonProperty("all")]
    public int Cloudiness { get; set; }
}