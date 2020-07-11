using Newtonsoft.Json;

namespace Goblin.OpenWeatherMap.Models.Current
{
    public class CurrentWeatherData
    {
        /// <summary>
        /// Атмосферное давление на уровне земли, гПа (гектопаскали)
        /// </summary>
        [JsonProperty("grnd_level", NullValueHandling = NullValueHandling.Ignore)]
        public double? GroundLevel { get; set; }

        /// <summary>
        /// Атмосферное давление на уровне моря, гПа (гектопаскали)
        /// </summary>
        [JsonProperty("sea_level", NullValueHandling = NullValueHandling.Ignore)]
        public double? SeaLevel { get; set; }
        
        /// <summary>
        /// Температура
        /// </summary>
        [JsonProperty("temp")]
        public double Temperature { get; set; }

        /// <summary>
        /// Атмосферное давление (на уровне моря, если нет данных о <see cref="GroundLevel"/> или <see cref="SeaLevel"/>)
        /// </summary>
        [JsonProperty("pressure")]
        public double Pressure { get; set; }

        /// <summary>
        /// Влажность
        /// </summary>
        [JsonProperty("humidity")]
        public double Humidity { get; set; }

        /// <summary>
        /// Минимальная температура
        /// Это минимальная наблюдаемая в настоящее время температура (в пределах крупных мегаполисов и городских районов).
        /// </summary>
        [JsonProperty("temp_min")]
        public double MinTemp { get; set; }

        /// <summary>
        /// Максимальная температура
        /// Это минимальная наблюдаемая в настоящее время температура (в пределах крупных мегаполисов и городских районов).
        /// </summary>
        [JsonProperty("temp_max")]
        public double MaxTemp { get; set; }
        
        /// <summary>
        /// Температура, которая учитывает восприятие погоды человеком
        /// </summary>
        [JsonProperty("feels_like", NullValueHandling = NullValueHandling.Ignore)]
        public double? FeelsLike { get; set; }
    }
}