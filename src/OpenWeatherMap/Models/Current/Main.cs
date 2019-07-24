using Newtonsoft.Json;

namespace OpenWeatherMap.Models.Current
{
    public class Main
    {
        [JsonProperty("temp")]
        public double Temperature { get; set; }

        [JsonProperty("pressure")]
        public double Pressure { get; set; }

        [JsonProperty("humidity")]
        public double Humidity { get; set; }

        [JsonProperty("temp_min")]
        public double MinTemp { get; set; }

        [JsonProperty("temp_max")]
        public double MaxTemp { get; set; }
    }
}