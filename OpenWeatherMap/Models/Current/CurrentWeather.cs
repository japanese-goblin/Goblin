using Newtonsoft.Json;

namespace OpenWeatherMap.Models.Current
{
    internal class CurrentWeather
    {
        [JsonProperty("coord")]
        private Coord Coord { get; set; }

        [JsonProperty("weather")]
        public Weather[] Info { get; set; }

        [JsonProperty("base")]
        public string BaseInfo { get; set; }

        [JsonProperty("main")]
        public Main Weather { get; set; }

        [JsonProperty("visibility")]
        public double Visibility { get; set; }

        [JsonProperty("wind")]
        public Wind Wind { get; set; }

        [JsonProperty("clouds")]
        public Clouds Clouds { get; set; }

        [JsonProperty("dt")]
        public int UnixTime { get; set; }

        [JsonProperty("sys")]
        public Sys OtherInfo { get; set; }

        [JsonProperty("id")]
        private int Id { get; set; }

        [JsonProperty("name")]
        public string CityName { get; set; }

        [JsonProperty("cod")]
        public int CityCode { get; set; }
    }
}