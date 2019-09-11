using System;
using Flurl.Http;
using Serilog;

namespace Goblin.OpenWeatherMap
{
    internal static class Defaults
    {
        public const string EndPoint = "https://api.openweathermap.org/data/2.5/";

        private const string Language = "ru";
        private const string Units = "metric";

        internal const double PressureConvert = 0.75006375541921;

        internal const int MaxDailyWeatherDifference = 16;

        private static IFlurlClient _client;
        private static ILogger _logger;

        internal static IFlurlRequest BuildRequest(string token)
        {
            if(_client is null)
            {
                _logger = Log.ForContext<OpenWeatherMapApi>();
                _client = new FlurlClient { BaseUrl = EndPoint };
                _client.WithTimeout(5)
                       .WithHeaders(new
                       {
                           Accept = "application/json",
                           User_Agent = "Japanese Goblin 2.0"
                       });

                _client.Settings.BeforeCall = call => _logger.Debug("Запрос [{0}] {1}",
                                                                    call.Request.Method, call.Request.RequestUri);
                _client.Settings.AfterCall = call => _logger.Debug("Запрос выполнен за {0}", call.Duration);
                _client.Settings.OnError = call => _logger.Error(call.Exception, "Ошибка при выполнении запроса",
                                                                 call.Request.RequestUri);
            }

            return _client.Request()
                          .SetQueryParam("units", Units)
                          .SetQueryParam("appid", token)
                          .SetQueryParam("lang", Language);
        }

        internal static DateTime UnixToDateTime(double unixTimeStamp)
        {
            var dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }
    }
}