using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace OpenWeatherMap
{
    public class WeatherInfo
    {
        private const string EndPoint = "https://api.openweathermap.org/data/2.5";
        private readonly string Token;
        private readonly HttpClient Client;

        public WeatherInfo(string token)
        {
            Token = token;
            Client = new HttpClient();
        }

        public async Task<string> GetWeather(string city)
        {
            var req = $"weather?q={city}&units=metric&appid={Token}&lang=ru";
            var response = await Client.GetStringAsync($"{EndPoint}/{req}");
            var w = JsonConvert.DeserializeObject<Models.WeatherInfo>(response);

            // на {UnixToDateTime(w.UnixTime):dd.MM.yyyy HH:mm}
            var str = new StringBuilder();
            str.AppendFormat("Погода в городе {0} на данный момент:", city).AppendLine();
            str.AppendFormat("Температура: {0}", w.Weather.Temperature).AppendLine();
            str.AppendFormat("Описание погоды: {0}", w.Info[0].State).AppendLine();
            str.AppendFormat("Влажность: {0}", w.Weather.Humidity).AppendLine();
            str.AppendFormat("Ветер: {0}", w.Wind.SpeedInfo).AppendLine();
            str.AppendFormat("Давление: {0}", w.Weather.Pressure).AppendLine();
            str.AppendFormat("Облачность: {0}", w.Clouds.Cloudiness).AppendLine();
            str.AppendFormat("Видимость: {0}", w.Visibility).AppendLine();
            str.AppendLine();
            str.AppendFormat("Восход в {0:HH.mm}", w.OtherInfo.Sunrise).AppendLine();
            str.AppendFormat("Закат в {0:HH.mm}", w.OtherInfo.Sunset).AppendLine();

            return str.ToString();
        }

        public async Task<bool> CheckCity(string city)
        {
            var req = $"weather?q={city}&units=metric&appid={Token}";
            var r = await Client.GetAsync($"{EndPoint}/{req}");

            return r.IsSuccessStatusCode;
        }

        internal static DateTime UnixToDateTime(double unixTimeStamp)
        {
            var dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }
    }
}
