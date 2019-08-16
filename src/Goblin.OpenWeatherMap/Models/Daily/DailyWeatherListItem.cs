﻿using System.Text;
using Newtonsoft.Json;

namespace Goblin.OpenWeatherMap.Models.Daily
{
    public class DailyWeatherListItem
    {
        [JsonProperty("dt")]
        public long UnixTime { get; set; }

        [JsonProperty("temp")]
        public Temp Temp { get; set; }

        [JsonProperty("pressure")]
        public double Pressure { get; set; }

        [JsonProperty("humidity")]
        public long Humidity { get; set; }

        [JsonProperty("weather")]
        public Weather[] Weather { get; set; }

        [JsonProperty("speed")]
        public double Speed { get; set; }

        [JsonProperty("deg")]
        public long Deg { get; set; }

        [JsonProperty("clouds")]
        public long Cloudiness { get; set; }

        [JsonProperty("rain", NullValueHandling = NullValueHandling.Ignore)]
        public double? Rain { get; set; }

        [JsonProperty("snow", NullValueHandling = NullValueHandling.Ignore)]
        public double? Snow { get; set; }

        public override string ToString()
        {
            var strBuilder = new StringBuilder();
            
            strBuilder.AppendFormat("Температура: от {0:+#;-#;0}°С до {1:+#;-#;0}°С", Temp.Min,
                                    Temp.Max).AppendLine();
            strBuilder.AppendFormat("Температура ночью: {0:+#;-#;0}", Temp.Night).AppendLine();
            strBuilder.AppendFormat("Температура утром: {0:+#;-#;0}", Temp.Morning).AppendLine();
            strBuilder.AppendFormat("Температура днем: {0:+#;-#;0}", Temp.Day).AppendLine();
            strBuilder.AppendFormat("Температура вечером: {0:+#;-#;0}", Temp.Evening).AppendLine();
            strBuilder.AppendLine();
            strBuilder.AppendFormat("Описание погоды: {0}", Weather[0].State).AppendLine();
            strBuilder.AppendFormat("Влажность: {0}%", Humidity).AppendLine();
            strBuilder.AppendFormat("Ветер: {0:N0} м/с", Speed).AppendLine();
            strBuilder.AppendFormat("Давление: {0:N0} мм.рт.ст", Pressure * Defaults.PressureConvert).AppendLine();
            strBuilder.AppendFormat("Облачность: {0}%", Cloudiness).AppendLine();

            return strBuilder.ToString();
        }
    }
}