using Flurl.Http;
using Serilog;

namespace Goblin.Narfu
{
    public static class Defaults
    {
        public const string EndPoint = "https://ruz.narfu.ru/";

        public const string UserAgent =
                "Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/76.0.3809.100 Safari/537.36";

        private static IFlurlClient _client;
        private static ILogger _logger;

        internal static IFlurlRequest BuildRequest()
        {
            if(_client is null)
            {
                _logger = Log.ForContext<NarfuApi>();
                _client = new FlurlClient { BaseUrl = EndPoint };
                _client.WithTimeout(5)
                       .WithHeader("User-Agent", UserAgent);

                _client.Settings.BeforeCall = call => _logger.Debug("Запрос [{0}] {1}",
                                                                    call.Request.Method, call.Request.RequestUri);
                _client.Settings.AfterCall = call => _logger.Debug("Запрос выполнен за {0}", call.Duration);
                _client.Settings.OnError = call => _logger.Error(call.Exception, "Ошибка при выполнении запроса",
                                                                 call.Request.RequestUri);
            }

            return _client.Request();
        }
    }
}