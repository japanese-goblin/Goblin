using Flurl.Http;
using Serilog;

namespace Goblin.OpenWeatherMap
{
    public static class RequestBuilder
    {
        private static IFlurlClient _client;
        private static ILogger _logger;

        internal static IFlurlRequest Create(string token)
        {
            if(_client is null)
            {
                _logger = Log.ForContext<OpenWeatherMapApi>();
                _client = new FlurlClient { BaseUrl = Defaults.EndPoint };
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
                          .SetQueryParam("units", Defaults.Units)
                          .SetQueryParam("appid", token)
                          .SetQueryParam("lang", Defaults.Language);
        }
    }
}