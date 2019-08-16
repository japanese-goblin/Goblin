using Newtonsoft.Json;

namespace Goblin.OpenWeatherMap.Models
{
    public class Coord
    {
        [JsonProperty("lon")]
        public double Longitude { get; set; }

        [JsonProperty("lat")]
        public double Latitude { get; set; }
    }
}