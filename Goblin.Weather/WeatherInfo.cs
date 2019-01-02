using Newtonsoft.Json;
using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Goblin.Weather
{
    public static class WeatherInfo
    {
        private const string EndPoint = "http://api.openweathermap.org/data/2.5";
        private const string Token = "***REMOVED***";
        private static readonly WebClient Client = new WebClient();

        public static async Task<string> GetWeather(string city)
        {
            Client.Encoding = Encoding.UTF8;
            var req = $"weather?q={city}&units=metric&appid={Token}&lang=ru";
            var response = await Client.DownloadStringTaskAsync($"{EndPoint}/{req}");
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
            var result = false;
            var req = $"weather?q={city}&units=metric&appid={Token}";
            try
            {
                var response = await Client.DownloadStringTaskAsync($"{EndPoint}/{req}");
                var w = JsonConvert.DeserializeObject<Models.WeatherInfo>(response);
                result = true;
            }
            catch
            {
            }

            return result;
        }

        public static DateTime UnixToDateTime(double unixTimeStamp)
        {
            var dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }
    }
}