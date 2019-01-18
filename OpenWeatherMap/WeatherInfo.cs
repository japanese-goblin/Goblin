using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace OpenWeatherMap
{
    public static class WeatherInfo
    {
        private const string EndPoint = "http://api.openweathermap.org/data/2.5";
        private const string Token = "***REMOVED***";
        private static readonly HttpClient Client = new HttpClient();

        public static async Task<string> GetWeather(string city)
        {
            var req = $"weather?q={city}&units=metric&appid={Token}&lang=ru";
            var response = await Client.GetStringAsync($"{EndPoint}/{req}");
            var w = JsonConvert.DeserializeObject<Models.WeatherInfo>(response);
            // на {UnixToDateTime(w.UnixTime):dd.MM.yyyy HH:mm}
            var result = $"Погода в городе {city} на данный момент:\n" +
                         $"Температура: {w.Weather.Temperature}\n" +
                         $"Описание погоды: {w.Info[0].State}\n" +
                         $"Влажность: {w.Weather.Humidity}\n" +
                         $"Ветер: {w.Wind.SpeedInfo}\n" +
                         $"Давление: {w.Weather.Pressure}\n" +
                         $"Облачность: {w.Clouds.Cloudiness}\n" +
                         $"Видимость: {w.Visibility}\n\n" +
                         $"Восход в {w.OtherInfo.Sunrise:HH:mm}\n" +
                         $"Закат в {w.OtherInfo.Sunset:HH:mm}";

            return result;
        }

        public static async Task<bool> CheckCity(string city)
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