using Newtonsoft.Json;

namespace Goblin.OpenWeatherMap.Models
{
    public class Weather
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("main")]
        private string Main { get; set; }

        [JsonProperty("description")]
        public string State { get; set; }

        [JsonProperty("icon")]
        public string Icon { get; set; }
    }
}