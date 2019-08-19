using System;
using System.Linq;
using System.Threading.Tasks;
using Goblin.Application.Extensions;
using Goblin.DataAccess;
using Goblin.OpenWeatherMap;
using Microsoft.EntityFrameworkCore;
using VkNet.Abstractions;
using VkNet.Model.RequestParams;

namespace Goblin.Application.Hangfire
{
    public class WeatherTask
    {
        private readonly OpenWeatherMapApi _weatherApi;
        private readonly BotDbContext _db;
        private readonly IVkApi _vkApi;

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
                    try
                    {
                        var weather = await _weatherApi.GetDailyWeatherAt(group.Key, DateTime.Today);
                        await _vkApi.Messages.SendToUserIdsWithRandomId(new MessagesSendParams
                        {
                            Message = $"Погода в городе {group.Key} на сегодня ({DateTime.Now:dddd, dd.MM.yyyy}):\n{weather}",
                            UserIds = ids
                        });
                    }
                    catch
                    {
                        await _vkApi.Messages.SendErrorToUserIds("Невозможно получить погоду с сайта.", ids);
                    }

                    await Task.Delay(Defaults.ExtraDelay);
                }
            }
        }
    }
}