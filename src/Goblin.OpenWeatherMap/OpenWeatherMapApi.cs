using System;
using System.Linq;
using System.Threading.Tasks;
using Flurl.Http;
using Flurl.Http.Configuration;
using Goblin.OpenWeatherMap.Models.Current;
using Goblin.OpenWeatherMap.Models.Daily;
using Serilog;

namespace Goblin.OpenWeatherMap
{
    public class OpenWeatherMapApi
    {
        private static string _token;
        private readonly ILogger _logger;
        private readonly IFlurlClient _client;

        public OpenWeatherMapApi(IFlurlClientFactory clientFactory)
        {
            _logger = Log.ForContext<OpenWeatherMapApi>();
            _logger.Debug("Инициализация {0}", nameof(OpenWeatherMapApi));

            _client = clientFactory.Get(Defaults.EndPoint)
                                      .WithTimeout(5)
                                      .WithHeaders(new
                                      {
                                          Accept = "application/json",
                                          User_Agent = Defaults.UserAgent
                                      });
            _client.Settings.BeforeCall = call => _logger.Debug("Запрос [{0}] {1}",
                                                               call.Request.Method, call.Request.RequestUri);
            _client.Settings.AfterCall = call => _logger.Debug("Запрос выполнен за {0}", call.Duration);
            _client.Settings.OnError = call => _logger.Error(call.Exception, "Ошибка при выполнении запроса {2}",
                                                            call.Request.RequestUri);
        }

        public async Task<CurrentWeather> GetCurrentWeather(string city)
        {
            _logger.Debug("Получение погоды на текущий момент в городе {0}", city);
            var response = await BuildRequest()
                                 .AppendPathSegment("weather")
                                 .SetQueryParam("q", city)
                                 .GetJsonAsync<CurrentWeather>();
            _logger.Debug("Погода получена");

            return response;
        }

        public async Task<DailyWeatherListItem> GetDailyWeatherAt(string city, DateTime date)
        {
            _logger.Debug("Получение погоды на день в городе {0} на дату {1:dd.MM.yyyy}", city, date);
            var dif = date.Date - DateTime.Today;
            if(dif > TimeSpan.FromDays(Defaults.MaxDailyWeatherDifference))
            {
                var msg = $"Погоду можно получить максимум на {Defaults.MaxDailyWeatherDifference} дней";
                _logger.Error(msg);
                throw new ArgumentException(msg);
            }

            var count = dif.Days + 1;

            var response = await BuildRequest()
                                 .AppendPathSegment("forecast/daily")
                                 .SetQueryParam("q", city)
                                 .SetQueryParam("cnt", count)
                                 .GetJsonAsync<DailyWeather>();
            var weather = response.List.FirstOrDefault(x => Defaults.UnixToDateTime(x.UnixTime).Date == date.Date);
            if(weather is null)
            {
                var msg = $"Погода на {date:dd.MM.yyyy} не найдена.";
                _logger.Error(msg);
                throw new ArgumentException(msg);
            }

            _logger.Debug("Погода получена");

            return weather;
        }

        public async Task<bool> IsCityExists(string city)
        {
            _logger.Debug("Проверка на существование города {0}", city);
            var response = await BuildRequest()
                                 .AppendPathSegment("weather")
                                 .SetQueryParam("q", city)
                                 .HeadAsync();

            return response.IsSuccessStatusCode;
        }

        private IFlurlRequest BuildRequest()
        {
            return _client.Request().SetQueryParam("units", Defaults.Units)
                          .SetQueryParam("appid", _token)
                          .SetQueryParam("lang", Defaults.Language)
                          .AllowAnyHttpStatus();
        }

        public static void SetToken(string token) //TODO:
        {
            if(!string.IsNullOrEmpty(_token))
            {
                throw new ArgumentException("Токен уже установлен");
            }
            
            if(string.IsNullOrWhiteSpace(token))
            {
                throw new ArgumentNullException("Токен пуст");
            }

            _token = token;
        }
    }
}