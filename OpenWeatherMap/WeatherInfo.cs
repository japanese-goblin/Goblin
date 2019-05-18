using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Flurl;
using Flurl.Http;
using OpenWeatherMap.Models.Current;
using OpenWeatherMap.Models.Daily;

namespace OpenWeatherMap
{
    public class WeatherInfo
    {
        public const string EndPoint = "https://api.openweathermap.org/data/2.5/";

        private const string Language = "ru";
        private const string Units = "metric";
        private readonly IFlurlRequest _request;

        public WeatherInfo(string token)
        {
            if(string.IsNullOrEmpty(token))
            {
                throw new ArgumentNullException(nameof(token), "Токен отсутствует");
            }

            _request = EndPoint.SetQueryParam("units", Units)
                              .SetQueryParam("appid", token)
                              .SetQueryParam("lang", Language)
                              .WithHeaders(new
                              {
                                  Accept = "application/json",
                                  User_Agent = "Japanese Goblin 1.0"
                              })
                              .AllowAnyHttpStatus();
        }

        public async Task<CurrentWeather> GetCurrentWeather(string city)
        {
            city = char.ToUpper(city[0]) + city.Substring(1); //TODO ?
            var response = await _request.AppendPathSegment("weather")
                                        .SetQueryParam("q", city)
                                        .GetJsonAsync<CurrentWeather>();

            return response;
        }

        public async Task<string> GetCurrentWeatherString(string city)
        {
            var w = await GetCurrentWeather(city);

            // на {UnixToDateTime(w.UnixTime):dd.MM.yyyy HH:mm}
            const double pressureConvert = 0.75006375541921;

            var strBuilder = new StringBuilder();
            strBuilder.AppendFormat("Погода в городе {0} на данный момент:", city).AppendLine();
            strBuilder.AppendFormat("Температура: {0:+#;-#;0}°С", w.Weather.Temperature).AppendLine();
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

        public async Task<DailyWeather> GetDailyWeather(string city, DateTime date)
        {
            const int count = 2;
            city = char.ToUpper(city[0]) + city.Substring(1); //TODO ?

            var response = await _request.AppendPathSegment("forecast/daily")
                                        .SetQueryParam("q", city)
                                        .SetQueryParam("cnt", count)
                                        .GetJsonAsync<DailyWeather>();

            return response;
        }

        public async Task<string> GetDailyWeatherString(string city, DateTime date)
        {
            var w = await GetDailyWeather(city, date);
            var weatherToday = w.List.FirstOrDefault(x => UnixToDateTime(x.UnixTime).Date == DateTime.Today);
            if(weatherToday is null)
            {
                return $"Погода на {date:dd.MM} не найдена";
            }

            const double pressureConvert = 0.75006375541921;

            string day;
            if(date.Date == DateTime.Today.Date)
            {
                day = $"сегодня ({date:dddd, d MMMM})";
            }
            else if(date.Date == DateTime.Today.AddDays(1).Date)
            {
                day = $"завтра ({date:dddd, d MMMM})";
            }
            else
            {
                day = DateTime.Now.ToString("dddd, d MMMM");
            }

            var strBuilder = new StringBuilder();
            strBuilder.AppendFormat("Погода в городе {0} на {1}:", city, day).AppendLine();

            strBuilder.AppendFormat("Температура: от {0:+#;-#;0}°С до {1:+#;-#;0}°С", weatherToday.Temp.Min,
                                    weatherToday.Temp.Max).AppendLine();
            strBuilder.AppendFormat("Температура ночью: {0:+#;-#;0}", weatherToday.Temp.Night).AppendLine();
            strBuilder.AppendFormat("Температура утром: {0:+#;-#;0}", weatherToday.Temp.Morning).AppendLine();
            strBuilder.AppendFormat("Температура днем: {0:+#;-#;0}", weatherToday.Temp.Day).AppendLine();
            strBuilder.AppendFormat("Температура вечером: {0:+#;-#;0}", weatherToday.Temp.Evening).AppendLine();
            strBuilder.AppendLine();

            strBuilder.AppendFormat("Описание погоды: {0}", weatherToday.Weather[0].State).AppendLine();
            strBuilder.AppendFormat("Влажность: {0}%", weatherToday.Humidity).AppendLine();
            strBuilder.AppendFormat("Ветер: {0:N0} м/с", weatherToday.Speed).AppendLine();
            strBuilder.AppendFormat("Давление: {0:N0} мм.рт.ст", weatherToday.Pressure * pressureConvert).AppendLine();
            strBuilder.AppendFormat("Облачность: {0}%", weatherToday.Cloudiness).AppendLine();

            return strBuilder.ToString();
        }

        public async Task<bool> CheckCity(string city)
        {
            var response = await _request.AppendPathSegment("weather")
                                        .SetQueryParam("q", city)
                                        .GetAsync();

            return response.IsSuccessStatusCode;
        }

        internal DateTime UnixToDateTime(double unixTimeStamp)
        {
            var dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }
    }
}