using System;
using System.Linq;
using System.Threading.Tasks;
using Goblin.Application.Core.Extensions;
using Goblin.Application.Core.Services;
using Goblin.Application.Vk.Extensions;
using Goblin.DataAccess;
using Serilog;
using VkNet.Abstractions;
using VkNet.Model.RequestParams;

namespace Goblin.Application.Vk.Hangfire
{
    public class WeatherTask
    {
        private readonly BotDbContext _db;
        private readonly ILogger _logger;
        private readonly IVkApi _vkApi;
        private readonly WeatherService _weatherService;

        public WeatherTask(WeatherService weatherService, BotDbContext db, IVkApi vkApi)
        {
            _weatherService = weatherService;
            _db = db;
            _vkApi = vkApi;
            _logger = Log.ForContext<WeatherTask>();
        }

        public async Task SendDailyWeather()
        {
            var grouped = _db.VkBotUsers.Where(x => x.HasWeatherSubscription)
                             .ToArray()
                             .GroupBy(x => x.WeatherCity);
            foreach(var group in grouped)
            {
                foreach(var chunk in group.Chunk(Defaults.ChunkLimit))
                {
                    try
                    {
                        var ids = chunk.Select(x => x.Id).ToArray();
                        var result = await _weatherService.GetDailyWeather(group.Key, DateTime.Today);

                        await _vkApi.Messages.SendToUserIdsWithRandomId(new MessagesSendParams
                        {
                            UserIds = ids,
                            Message = result.Message
                        });

                        await Task.Delay(Defaults.ExtraDelay);
                    }
                    catch(Exception ex)
                    {
                        _logger.Error(ex, "Ошибка при отправке погоды");
                    }
                }
            }
        }
    }
}