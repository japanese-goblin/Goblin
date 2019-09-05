using Flurl.Http;
using Flurl.Http.Configuration;
using Goblin.Narfu.Schedule;
using Serilog;

namespace Goblin.Narfu
{
    public class NarfuApi
    {
        public TeachersSchedule Teachers { get; }
        public StudentsSchedule Students { get; }

        private const string EndPoint = "https://ruz.narfu.ru/";

        private const string UserAgent =
                "Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/76.0.3809.100 Safari/537.36";

        private readonly ILogger _logger;

        public NarfuApi(IFlurlClientFactory clientFactory)
        {
            _logger = Log.ForContext<NarfuApi>();

            var client = clientFactory.Get(EndPoint)
                                      .WithTimeout(5)
                                      .WithHeader("User-Agent", UserAgent);
            client.Settings.BeforeCall = call => _logger.Debug("Запрос [{0}] {1}",
                                                               call.Request.Method, call.Request.RequestUri);
            client.Settings.AfterCall = call => _logger.Debug("Запрос выполнен за {0}", call.Duration);
            client.Settings.OnError = call => _logger.Error(call.Exception, "Ошибка при выполнении запроса {2}",
                                                            call.Request.RequestUri);
            
            Teachers = new TeachersSchedule(client);
            Students = new StudentsSchedule(client);
        }
    }
}