using Newtonsoft.Json;

namespace Goblin.OpenWeatherMap.Models.Daily
{
    public class City
    {
        /// <summary>
        /// Идентификатор города
        /// </summary>
        [JsonProperty("id")]
        public long Id { get; set; }

        /// <summary>
        /// Название города
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// Координаты города
        /// </summary>
        [JsonProperty("coord")]
        public Coordinates Coordinates { get; set; }

        /// <summary>
        /// Страна, в котором расположен город
        /// </summary>
        [JsonProperty("country")]
        public string Country { get; set; }

        /// <summary>
        /// Население города
        /// </summary>
        [JsonProperty("population")]
        public long Population { get; set; }
    }
}