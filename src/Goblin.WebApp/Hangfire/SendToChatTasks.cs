using System;
using System.Threading.Tasks;
using Goblin.Application.Core.Abstractions;
using Goblin.Application.Core.Options;
using Goblin.Application.Vk.Extensions;
using Goblin.Domain;
using Microsoft.Extensions.Options;
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
        private readonly IWeatherService _weatherService;
        private readonly MailingOptions _mailingOptions;
        
        public SendToChatTasks(IScheduleService scheduleService, IWeatherService weatherService, IVkApi vkApi,
                               TelegramBotClient botClient, IOptions<MailingOptions> mailingOptions)
        {
            _scheduleService = scheduleService;
            _weatherService = weatherService;
            _vkApi = vkApi;
            _botClient = botClient;
            _mailingOptions = mailingOptions.Value;
        }

        public async Task SendToConv(long id, int group = 0, string city = "", ConsumerType type = ConsumerType.Vkontakte)
        {
            if(type == ConsumerType.Vkontakte)
            {
                await Send(SendToVk);
            }
            else if(type == ConsumerType.Telegram)
            {
                await Send(SendToTelegram);
            }

            async Task Send(Func<string, Task> func)
            {
                await SendWeather(id, city, func);
                await SendSchedule(id, group, func);
            }

            async Task SendToVk(string message)
            {
                await _vkApi.Messages.SendWithRandomId(new MessagesSendParams
                {
                    PeerId = id,
                    Message = message
                });
            }
            
            async Task SendToTelegram(string message)
            {
                await _botClient.SendTextMessageAsync(id, message);
            }
        }

        private async Task SendWeather(long id, string city, Func<string, Task> send)
        {
            if(string.IsNullOrWhiteSpace(city))
            {
                return;
            }

            Log.Information("Отправка погоды в {0}", id);
            var result = await _weatherService.GetDailyWeather(city, DateTime.Today);
            
            await send(result.Message);
        }

        private async Task SendSchedule(long id, int group, Func<string, Task> send)
        {
            var isSunday = DateTime.Today.DayOfWeek == DayOfWeek.Sunday;
            if(group == 0 || isSunday)
            {
                return;
            }

            Log.Information("Отправка расписания в {0}", id);
            var result = await _scheduleService.GetSchedule(group, DateTime.Now);
            if(!result.IsSuccessful && _mailingOptions.IsVacations)
            {
                return;
            }
            
            await send(result.Message);
        }
    }
}