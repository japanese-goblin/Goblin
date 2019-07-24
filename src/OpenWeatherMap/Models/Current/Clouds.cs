using Newtonsoft.Json;

namespace OpenWeatherMap.Models.Current
{
    public class Clouds
    {
        [JsonProperty("all")]
        public int Cloudiness { get; set; }
    }
}