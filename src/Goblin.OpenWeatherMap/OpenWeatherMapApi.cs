using System;
using System.Linq;
using System.Threading.Tasks;
using Flurl.Http;
using Goblin.OpenWeatherMap.Models.Current;
using Goblin.OpenWeatherMap.Models.Daily;
using Serilog;

namespace Goblin.OpenWeatherMap
{
    public class OpenWeatherMapApi
    {
        private readonly string _token;
        private readonly ILogger _logger;

        public OpenWeatherMapApi(string token)
        {
            _logger = Log.ForContext<OpenWeatherMapApi>();

            _logger.Debug("Инициализация {0}", nameof(OpenWeatherMapApi));
            if(string.IsNullOrWhiteSpace(token))
            {
                _logger.Fatal("Токен пуст", nameof(OpenWeatherMapApi));
                throw new ArgumentException("Токен пуст");
            }

            _token = token;
        }

        public async Task<CurrentWeather> GetCurrentWeather(string city)
        {
            _logger.Debug("Получение погоды на текущий момент в городе {0}", city);
            var response = await RequestBuilder.Create(_token)
                                               .AppendPathSegment("weather")
                                               .SetQueryParam("q", city)
                                               .GetJsonAsync<CurrentWeather>();
            _logger.Debug("Погода получена");

            return response;
        }

        public async Task<DailyWeatherListItem> GetDailyWeatherAt(string city, DateTime date)
        {
            _logger.Debug("Получение погоды на день в городе {0} на дату {1:dd.MM.yyyy}", city, date);

            var response = await RequestBuilder.Create(_token)
                                               .AppendPathSegment("forecast/daily")
                                               .SetQueryParam("q", city)
                                               .SetQueryParam("cnt", 4)
                                               .GetJsonAsync<DailyWeather>();
            var weather = response.List.FirstOrDefault(x => Defaults.UnixToDateTime(x.UnixTime).Date == date.Date);
            if(weather is null)
            {
                var msg = $"Погода на {date:dd.MM.yyyy} в городе {city} не найдена.";
                _logger.Error(msg);
                throw new ArgumentException(msg);
            }

            _logger.Debug("Погода получена");

            return weather;
        }

        public async Task<bool> IsCityExists(string city)
        {
            _logger.Debug("Проверка на существование города {0}", city);
            var response = await RequestBuilder.Create(_token)
                                               .AllowAnyHttpStatus()
                                               .AppendPathSegment("weather")
                                               .SetQueryParam("q", city)
                                               .HeadAsync();

            return response.IsSuccessStatusCode;
        }
    }
}