using Newtonsoft.Json;

namespace Goblin.OpenWeatherMap.Models.Current
{
    public class Rain
    {
        /// <summary>
        /// Количество осадков за последний час
        /// </summary>
        [JsonProperty("1h", NullValueHandling = NullValueHandling.Ignore)]
        public double? ForLastOneHour { get; set; }

        /// <summary>
        /// Количество осадков за последние три часа
        /// </summary>
        [JsonProperty("3h", NullValueHandling = NullValueHandling.Ignore)]
        public double? ForLastThreeHours { get; set; }
    }
}