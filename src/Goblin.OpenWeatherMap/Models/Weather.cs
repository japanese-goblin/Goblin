using Newtonsoft.Json;

namespace Goblin.OpenWeatherMap.Models
{
    public class Weather
    {
        /// <summary>
        /// Описание погоды
        /// </summary>
        [JsonProperty("description")]
        public string Description { get; set; }

        /// <summary>
        /// Иконка погоды
        /// </summary>
        [JsonProperty("icon", NullValueHandling = NullValueHandling.Ignore)]
        public string Icon { get; set; }

        /// <summary>
        /// Идентификатор состояния погоды
        /// </summary>
        [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
        public long? Id { get; set; }

        /// <summary>
        /// Группа погодных условий (?)
        /// </summary>
        [JsonProperty("main")]
        public string Main { get; set; }
    }
}