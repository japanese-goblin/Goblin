using System;
using System.Text;
using Goblin.OpenWeatherMap.Models.Current;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Goblin.OpenWeatherMap.Models.Responses
{
    public class CurrentWeatherResponse
    {
        /// <summary>
        /// Координаты
        /// </summary>
        [JsonProperty("coord")]
        public Coordinates Coordinates { get; set; }

        /// <summary>
        /// Внутреннее описание погоды
        /// </summary>
        [JsonProperty("weather")]
        public Weather[] Info { get; set; }

        /// <summary>
        /// Внутренний параметр (возможно, откуда берутся данные)
        /// </summary>
        [JsonProperty("base")]
        public string Base { get; set; }

        /// <summary>
        /// Данные о текущей погоде
        /// </summary>
        [JsonProperty("main")]
        public CurrentWeatherData Weather { get; set; }

        /// <summary>
        /// Дальность видимости (в метрах)
        /// </summary>
        [JsonProperty("visibility")]
        public double Visibility { get; set; }

        /// <summary>
        /// Информация о ветре
        /// </summary>
        [JsonProperty("wind")]
        public Wind Wind { get; set; }

        /// <summary>
        /// Информация об облаках
        /// </summary>
        [JsonProperty("clouds")]
        public Clouds Clouds { get; set; }
        
        /// <summary>
        /// Информация о дожде
        /// </summary>
        [JsonProperty("rain", NullValueHandling = NullValueHandling.Ignore)]
        public Rain Rain { get; set; }
        
        /// <summary>
        /// Информация о снеге
        /// </summary>
        [JsonProperty("snow", NullValueHandling = NullValueHandling.Ignore)]
        public Snow Snow { get; set; }

        /// <summary>
        /// Время получения погоды со станции
        /// </summary>
        [JsonProperty("dt")]
        [JsonConverter(typeof(UnixDateTimeConverter))]
        public DateTimeOffset UnixTime { get; set; }

        /// <summary>
        /// Системная информация
        /// </summary>
        [JsonProperty("sys")]
        public Sys OtherInfo { get; set; }

        /// <summary>
        /// Идентификатор горожа
        /// </summary>
        [JsonProperty("id")]
        public int CityId { get; set; }

        /// <summary>
        /// Название города
        /// </summary>
        [JsonProperty("name")]
        public string CityName { get; set; }

        /// <summary>
        /// Код ответа
        /// </summary>
        [JsonProperty("cod")]
        public int ResponseCode { get; set; }

        public override string ToString()
        {
            var strBuilder = new StringBuilder();

            strBuilder.AppendFormat("Погода в городе {0} на данный момент:", CityName).AppendLine();
            strBuilder.AppendFormat("Температура: {0:+#;-#;0}°С", Weather.Temperature).AppendLine();
            strBuilder.AppendFormat("Описание погоды: {0}", Info[0].Description).AppendLine();
            strBuilder.AppendFormat("Влажность: {0}%", Weather.Humidity).AppendLine();
            strBuilder.AppendFormat("Ветер: {0:N0} м/с", Wind.Speed).AppendLine();
            strBuilder.AppendFormat("Давление: {0:N0} мм.рт.ст", Weather.Pressure * Defaults.PressureConvert)
                      .AppendLine();
            strBuilder.AppendFormat("Облачность: {0}%", Clouds.Cloudiness).AppendLine();
            strBuilder.AppendFormat("Видимость: {0} метров", Visibility).AppendLine();

            return strBuilder.ToString();
        }
    }
}