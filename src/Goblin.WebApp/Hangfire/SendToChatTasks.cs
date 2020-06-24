using System;
using System.Threading.Tasks;
using Goblin.Application.Core.Abstractions;
using Goblin.Application.Core.Services;
using Goblin.Application.Vk.Extensions;
using Goblin.Domain;
using Serilog;
using Telegram.Bot;
using VkNet.Abstractions;
using VkNet.Model.RequestParams;

namespace Goblin.WebApp.Hangfire
{
    public class SendToChatTasks
    {
        private readonly TelegramBotClient _botClient;
        private readonly IScheduleService _scheduleService;
        private readonly IVkApi _vkApi;
        private readonly WeatherService _weatherService;

        public SendToChatTasks(IScheduleService scheduleService, WeatherService weatherService, IVkApi vkApi,
                               TelegramBotClient botClient)
        {
            _scheduleService = scheduleService;
            _weatherService = weatherService;
            _vkApi = vkApi;
            _botClient = botClient;
        }

        public async Task SendToConv(long id, int group = 0, string city = "", ConsumerType type = ConsumerType.Vkontakte)
        {
            if(type == ConsumerType.Vkontakte)
            {
                await SendToVk(id, group, city);
            }
            else if(type == ConsumerType.Telegram)
            {
                await SendToTelegram(id, group, city);
            }
        }

        private async Task SendToVk(long id, int group, string city)
        {
            if(!string.IsNullOrWhiteSpace(city))
            {
                Log.Information("Отправка погоды в {0}", id);
                var result = await _weatherService.GetDailyWeather(city, DateTime.Today);

                await _vkApi.Messages.SendWithRandomId(new MessagesSendParams
                {
                    PeerId = id,
                    Message = result.Message
                });
            }

            if(DateTime.Today.DayOfWeek != DayOfWeek.Sunday)
            {
                Log.Information("Отправка расписания в {0}", id);
                var result = await _scheduleService.GetSchedule(group, DateTime.Now);
                await _vkApi.Messages.SendWithRandomId(new MessagesSendParams
                {
                    PeerId = id,
                    Message = result.Message
                });
            }
        }

        private async Task SendToTelegram(long id, int group, string city)
        {
            if(!string.IsNullOrWhiteSpace(city))
            {
                Log.Information("Отправка погоды в {0}", id);
                var result = await _weatherService.GetDailyWeather(city, DateTime.Today);

                await _botClient.SendTextMessageAsync(id, result.Message);
            }

            if(DateTime.Today.DayOfWeek != DayOfWeek.Sunday)
            {
                Log.Information("Отправка расписания в {0}", id);
                var result = await _scheduleService.GetSchedule(group, DateTime.Now);
                await _botClient.SendTextMessageAsync(id, result.Message);
            }
        }
    }
}