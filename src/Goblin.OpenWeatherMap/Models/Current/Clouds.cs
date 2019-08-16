using Newtonsoft.Json;

namespace Goblin.OpenWeatherMap.Models.Current
{
    public class Clouds
    {
        [JsonProperty("all")]
        public int Cloudiness { get; set; }
    }
}