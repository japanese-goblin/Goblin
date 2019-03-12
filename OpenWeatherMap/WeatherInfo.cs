using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace OpenWeatherMap
{
    public class WeatherInfo
    {
        private const string EndPoint = "https://api.openweathermap.org/data/2.5/";
        private readonly string Token;
        private readonly HttpClient Client;

        public WeatherInfo(string token)
        {
            if(string.IsNullOrEmpty(token))
            {
                throw new ArgumentNullException(nameof(token), "Токен отсутствует");
            }

            Token = token;
            Client = new HttpClient
            {
                BaseAddress = new Uri(EndPoint)
            };
        }

        public async Task<string> GetWeather(string city)
        {
            city = char.ToUpper(city[0]) + city.Substring(1); //TODO ?
            var response = await Client.GetStringAsync($"weather?q={city}&units=metric&appid={Token}&lang=ru");
            var w = JsonConvert.DeserializeObject<Models.WeatherInfo>(response);

            // на {UnixToDateTime(w.UnixTime):dd.MM.yyyy HH:mm}
            const double pressureConvert = 0.75006375541921;
            
            var strBuilder = new StringBuilder();
            strBuilder.AppendFormat("Погода в городе {0} на данный момент:", city).AppendLine();
            strBuilder.AppendFormat("Температура: {0:N0}°С", w.Weather.Temperature).AppendLine();
            strBuilder.AppendFormat("Описание погоды: {0}", w.Info[0].State).AppendLine();
            strBuilder.AppendFormat("Влажность: {0}%", w.Weather.Humidity).AppendLine();
            strBuilder.AppendFormat("Ветер: {0:N0} м/с", w.Wind.Speed).AppendLine();
            strBuilder.AppendFormat("Давление: {0:N0} мм.рт.ст", w.Weather.Pressure * pressureConvert).AppendLine();
            strBuilder.AppendFormat("Облачность: {0}%", w.Clouds.Cloudiness).AppendLine();
            strBuilder.AppendFormat("Видимость: {0} метров", w.Visibility).AppendLine();
            strBuilder.AppendLine();
            strBuilder.AppendFormat("Восход в {0:HH:mm}", UnixToDateTime(w.OtherInfo.Sunrise)).AppendLine();
            strBuilder.AppendFormat("Закат в {0:HH:mm}", UnixToDateTime(w.OtherInfo.Sunset)).AppendLine();
            
            return strBuilder.ToString();
        }

        public async Task<bool> CheckCity(string city)
        {
            var req = $"weather?q={city}&units=metric&appid={Token}";
            var r = await Client.GetAsync($"{EndPoint}/{req}");

            return r.IsSuccessStatusCode;
        }

        internal DateTime UnixToDateTime(double unixTimeStamp)
        {
            var dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }
    }
}