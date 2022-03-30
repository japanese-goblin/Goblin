using System.Net;
using System.Threading.Tasks;
using Flurl.Http;
using Serilog;

namespace Goblin.Narfu;

public static class RequestBuilder
{
    private static IFlurlClient _client;
    private static ILogger _logger;

    internal static IFlurlRequest Create()
    {
        if(_client is null)
        {
            _logger = Log.ForContext<NarfuApi>();
            _client = new FlurlClient { BaseUrl = Defaults.EndPoint };
            _client.WithTimeout(5)
                   .WithHeader("User-Agent", Defaults.UserAgent);

            _client.Settings.BeforeCall = call => _logger.Debug("Запрос [{0}] {1}",
                                                                call.Request.Verb, call.Request.Url);
            _client.Settings.AfterCall = call => _logger.Debug("Запрос выполнен за {0}", call.Duration);
#if DEBUG
            _client.Settings.OnError = call => _logger.Error(call.Exception, "Ошибка при выполнении запроса");
#else
                _client.Settings.OnError = call =>
                {
                    if(call.HttpResponseMessage is null)
                    {
                        _logger.Error("[{0}] {1} - {2}", call.Request.Verb, call.Request.Url,
                                      call.HttpResponseMessage?.StatusCode ?? HttpStatusCode.GatewayTimeout);
                        return;
                    }
                    if(call.Exception is FlurlHttpException || call.Exception is TaskCanceledException)
                    {
                        return;
                    }

                    _logger.Error(call.Exception, "Ошибка при выполнении запроса");
                };
#endif
        }

        return _client.Request();
    }
}