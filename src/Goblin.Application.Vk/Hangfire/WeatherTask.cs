using System;
using System.Linq;
using System.Threading.Tasks;
using Goblin.Application.Core.Commands.Keyboard;
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
        private readonly WeatherDailyCommand _weatherDailyCommand;

        public WeatherTask(WeatherDailyCommand weatherDailyCommand, BotDbContext db, IVkApi vkApi)
        {
            _weatherDailyCommand = weatherDailyCommand;
            _db = db;
            _vkApi = vkApi;
            _logger = Log.ForContext<WeatherTask>();
        }

        public async Task SendDailyWeather()
        {
            var grouped = _db.BotUsers.Where(x => x.HasWeatherSubscription)
                             .ToArray()
                             .GroupBy(x => x.WeatherCity);
            foreach(var group in grouped)
            {
                foreach(var chunk in group.Chunk(Defaults.ChunkLimit))
                {
                    try
                    {
                        var ids = chunk.Select(x => x.Id).ToArray();
                        var result = await _weatherDailyCommand.GetDailyWeather(group.Key, DateTime.Today);

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