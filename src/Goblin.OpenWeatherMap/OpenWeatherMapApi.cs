using System;
using System.Linq;
using System.Threading.Tasks;
using Flurl.Http;
using Goblin.OpenWeatherMap.Models.Current;
using Goblin.OpenWeatherMap.Models.Daily;

namespace Goblin.OpenWeatherMap
{
    public class OpenWeatherMapApi
    {
        private readonly string _token;

        public OpenWeatherMapApi(string token)
        {
            if(string.IsNullOrWhiteSpace(token))
            {
                throw new ArgumentException("Токен пуст");
            }

            _token = token;
        }

        public async Task<CurrentWeather> GetCurrentWeather(string city)
        {
            var response = await Defaults.BuildRequest(_token)
                                         .AppendPathSegment("weather")
                                         .SetQueryParam("q", city)
                                         .GetJsonAsync<CurrentWeather>();
            return response;
        }

        public async Task<DailyWeatherListItem> GetDailyWeatherAt(string city, DateTime date)
        {
            var dif = date - DateTime.Now;
            if(dif > TimeSpan.FromDays(Defaults.MaxDailyWeatherDifference))
            {
                throw new
                        ArgumentException($"Погоду можно получить максимум на {Defaults.MaxDailyWeatherDifference} дней");
            }

            var count = dif.Days;

            var response = await Defaults.BuildRequest(_token)
                                         .AppendPathSegment("forecast/daily")
                                         .SetQueryParam("q", city)
                                         .SetQueryParam("cnt", count)
                                         .GetJsonAsync<DailyWeather>();
            var weather = response.List.SingleOrDefault(x => Defaults.UnixToDateTime(x.UnixTime) == date);
            if(weather is null)
            {
                throw new ArgumentException("Погода на указанную дату не найдена.");
            }

            return weather;
        }
    }
}