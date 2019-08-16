using Newtonsoft.Json;

namespace Goblin.OpenWeatherMap.Models.Current
{
    public class Wind
    {
        [JsonProperty("speed")]
        public double Speed { get; set; }

        [JsonProperty("deg")]
        public double Degrees { get; set; }
    }
}