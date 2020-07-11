using System;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Goblin.OpenWeatherMap.Models.Daily
{
    public class DailyWeatherListItem
    {
        /// <summary>
        /// Дата погоды
        /// </summary>
        [JsonProperty("dt")]
        [JsonConverter(typeof(UnixDateTimeConverter))]
        public DateTimeOffset UnixTime { get; set; }

        /// <summary>
        /// Температура
        /// </summary>
        [JsonProperty("temp")]
        public Temperature Temperature { get; set; }

        /// <summary>
        /// Температура с учётом человеческих ощущений
        /// </summary>
        [JsonProperty("feels_like")]
        public FeelsLike FeelsLike { get; set; }

        /// <summary>
        /// Атмосферное давление
        /// </summary>
        [JsonProperty("pressure")]
        public double Pressure { get; set; }

        /// <summary>
        /// Влажность
        /// </summary>
        [JsonProperty("humidity")]
        public long Humidity { get; set; }

        /// <summary>
        /// Внутреннее описание погоды
        /// </summary>
        [JsonProperty("weather")]
        public Weather[] Weather { get; set; }

        /// <summary>
        /// Скорость ветра
        /// </summary>
        [JsonProperty("speed")]
        public double WindSpeed { get; set; }

        /// <summary>
        /// Направление ветра, в градусах
        /// </summary>
        [JsonProperty("deg")]
        public long WindDeg { get; set; }

        /// <summary>
        /// Облачность, в %
        /// </summary>
        [JsonProperty("clouds")]
        public long Cloudiness { get; set; }

        /// <summary>
        /// Количество дождя, в миллиметрах
        /// </summary>
        [JsonProperty("rain", NullValueHandling = NullValueHandling.Ignore)]
        public double? Rain { get; set; }

        /// <summary>
        /// Количество снега, в миллиметрах
        /// </summary>
        [JsonProperty("snow", NullValueHandling = NullValueHandling.Ignore)]
        public double? Snow { get; set; }

        public override string ToString()
        {
            var strBuilder = new StringBuilder();

            strBuilder.AppendFormat("Температура: от {0:+#;-#;0}°С до {1:+#;-#;0}°С", Temperature.Min,
                                    Temperature.Max).AppendLine();
            strBuilder.AppendFormat("Температура утром: {0:+#;-#;0}", Temperature.Morning).AppendLine();
            strBuilder.AppendFormat("Температура днём: {0:+#;-#;0}", Temperature.Day).AppendLine();
            strBuilder.AppendFormat("Температура вечером: {0:+#;-#;0}", Temperature.Evening).AppendLine();
            strBuilder.AppendFormat("Температура ночью: {0:+#;-#;0}", Temperature.Night).AppendLine();
            strBuilder.AppendLine();
            strBuilder.AppendFormat("Описание погоды: {0}", Weather[0].Description).AppendLine();
            strBuilder.AppendFormat("Влажность: {0}%", Humidity).AppendLine();
            strBuilder.AppendFormat("Ветер: {0:N0} м/с", WindSpeed).AppendLine();
            strBuilder.AppendFormat("Давление: {0:N0} мм.рт.ст", Pressure * Defaults.PressureConvert).AppendLine();
            strBuilder.AppendFormat("Облачность: {0}%", Cloudiness).AppendLine();

            return strBuilder.ToString();
        }
    }
}