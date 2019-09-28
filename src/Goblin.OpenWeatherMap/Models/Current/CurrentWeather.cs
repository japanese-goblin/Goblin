using System.Text;
using Newtonsoft.Json;

namespace Goblin.OpenWeatherMap.Models.Current
{
    public class CurrentWeather
    {
        [JsonProperty("coord")]
        public Coord Coord { get; set; }

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
        public int ResponseCode { get; set; }

        public override string ToString()
        {
            var strBuilder = new StringBuilder();

            strBuilder.AppendFormat("Погода в городе {0} на данный момент:", CityName).AppendLine();
            strBuilder.AppendFormat("Температура: {0:+#;-#;0}°С", Weather.Temperature).AppendLine();
            strBuilder.AppendFormat("Описание погоды: {0}", Info[0].State).AppendLine();
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