using System;
using System.Linq;
using System.Threading.Tasks;
using Flurl.Http;
using Goblin.Application.Extensions;
using Goblin.Application.Results.Failed;
using Goblin.Application.Results.Success;
using Goblin.DataAccess;
using Goblin.OpenWeatherMap;
using Microsoft.EntityFrameworkCore;
using Serilog;
using VkNet.Abstractions;
using VkNet.Model.RequestParams;

namespace Goblin.Application.Hangfire
{
    public class WeatherTask
    {
        private readonly BotDbContext _db;
        private readonly IVkApi _vkApi;
        private readonly OpenWeatherMapApi _weatherApi;

        public WeatherTask(OpenWeatherMapApi weatherApi, BotDbContext db, IVkApi vkApi)
        {
            _weatherApi = weatherApi;
            _db = db;
            _vkApi = vkApi;
        }

        public async Task SendDailyWeather()
        {
            var grouped = _db.BotUsers.Include(x => x.SubscribeInfo)
                             .Where(x => x.SubscribeInfo.IsWeather)
                             .GroupBy(x => x.WeatherCity);
            foreach(var group in grouped)
            {
                foreach(var chunk in group.Chunk(Defaults.ChunkLimit))
                {
                    var ids = chunk.Select(x => x.VkId).ToArray();
                    var weather = await _weatherApi.GetDailyWeatherWithResult(group.Key, DateTime.Today);
                    if(weather is FailedResult failed)
                    {
                        await _vkApi.Messages.SendErrorToUserIds(failed.Error, ids);
                    }
                    else
                    {
                        var success = weather as SuccessfulResult;
                        await _vkApi.Messages.SendToUserIdsWithRandomId(new MessagesSendParams
                        {
                            UserIds = ids,
                            Message = success.Message
                        });
                    }

                    await Task.Delay(Defaults.ExtraDelay);
                }
            }
        }
    }
}