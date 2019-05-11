using Newtonsoft.Json;

namespace OpenWeatherMap.Models.Current
{
    public class Wind
    {
        [JsonProperty("speed")]
        public double Speed { get; set; }

        [JsonProperty("deg")]
        public double Degrees { get; set; }
    }
}