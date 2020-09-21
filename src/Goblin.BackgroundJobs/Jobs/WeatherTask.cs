using System;
using System.Linq;
using System.Threading.Tasks;
using Goblin.Application.Core.Abstractions;
using Goblin.Application.Core.Extensions;
using Goblin.Application.Vk.Extensions;
using Goblin.DataAccess;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using VkNet.Abstractions;
using VkNet.Model.RequestParams;

namespace Goblin.BackgroundJobs.Jobs
{
    public class WeatherTask
    {
        private readonly TelegramBotClient _botClient;
        private readonly BotDbContext _db;
        private readonly ILogger _logger;
        private readonly IVkApi _vkApi;
        private readonly IWeatherService _weatherService;

        public WeatherTask(IWeatherService weatherService, BotDbContext db, IVkApi vkApi, TelegramBotClient botClient)
        {
            _weatherService = weatherService;
            _db = db;
            _vkApi = vkApi;
            _botClient = botClient;
            _logger = Log.ForContext<WeatherTask>();
        }

        public async Task Execute()
        {
            await SendToVk();
            await SendToTelegram();
        }

        private async Task SendToVk()
        {
            var grouped = _db.VkBotUsers
                             .AsNoTracking()
                             .Where(x => x.HasWeatherSubscription)
                             .ToArray()
                             .GroupBy(x => x.WeatherCity);
            foreach(var group in grouped)
            {
                var result = await _weatherService.GetDailyWeather(group.Key, DateTime.Today);
                foreach(var chunk in group.Chunk(Defaults.ChunkLimit))
                {
                    try
                    {
                        var ids = chunk.Select(x => x.Id).ToArray();

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

        private async Task SendToTelegram()
        {
            var grouped = _db.TgBotUsers
                             .AsNoTracking()
                             .Where(x => x.HasWeatherSubscription)
                             .ToArray()
                             .GroupBy(x => x.WeatherCity);
            foreach(var group in grouped)
            {
                var result = await _weatherService.GetDailyWeather(group.Key, DateTime.Today);
                foreach(var user in group)
                {
                    try
                    {
                        await _botClient.SendTextMessageAsync(user.Id, result.Message);
                    }
                    catch(ApiRequestException ex)
                    {
                        if(ex.ErrorCode == 403) // Forbidden: bot was blocked by the user
                        {
                            _db.TgBotUsers.Remove(user);
                        }
                    }
                    catch(Exception ex)
                    {
                        _logger.Error(ex, "Ошибка при отправке погоды");
                    }
                }
            }

            await _db.SaveChangesAsync();
        }
    }
}