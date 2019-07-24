using Newtonsoft.Json;

namespace OpenWeatherMap.Models.Daily
{
    public class List
    {
        [JsonProperty("dt")]
        public long UnixTime { get; set; }

        [JsonProperty("temp")]
        public Temp Temp { get; set; }

        [JsonProperty("pressure")]
        public double Pressure { get; set; }

        [JsonProperty("humidity")]
        public long Humidity { get; set; }

        [JsonProperty("weather")]
        public Weather[] Weather { get; set; }

        [JsonProperty("speed")]
        public double Speed { get; set; }

        [JsonProperty("deg")]
        public long Deg { get; set; }

        [JsonProperty("clouds")]
        public long Cloudiness { get; set; }

        [JsonProperty("rain", NullValueHandling = NullValueHandling.Ignore)]
        public double? Rain { get; set; }

        [JsonProperty("snow", NullValueHandling = NullValueHandling.Ignore)]
        public double? Snow { get; set; }
    }
}