using Newtonsoft.Json;

namespace Goblin.OpenWeatherMap.Models.Current
{
    public class Wind
    {
        /// <summary>
        /// Направление ветра (в градусах)
        /// </summary>
        [JsonProperty("deg")]
        public double Degrees { get; set; }

        /// <summary>
        /// Порыв ветра
        /// </summary>
        [JsonProperty("gust", NullValueHandling = NullValueHandling.Ignore)]
        public double? Gust { get; set; }

        /// <summary>
        /// Скорость ветра
        /// </summary>
        [JsonProperty("speed")]
        public double Speed { get; set; }
    }
}