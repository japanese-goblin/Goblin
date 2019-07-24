using Newtonsoft.Json;

namespace OpenWeatherMap.Models
{
    public class Coord
    {
        [JsonProperty("lon")]
        public double Longitude { get; set; }

        [JsonProperty("lat")]
        public double Latitude { get; set; }
    }
}