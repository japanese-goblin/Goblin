using Newtonsoft.Json;

namespace OpenWeatherMap.Models.Current {
    internal class Clouds
    {
        [JsonProperty("all")]
        public int Cloudiness { get; set; }
    }
}