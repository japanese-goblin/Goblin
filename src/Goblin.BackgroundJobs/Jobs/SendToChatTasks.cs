using System;
using System.Threading.Tasks;
using Goblin.Application.Core.Abstractions;
using Goblin.Application.Core.Options;
using Goblin.Application.Vk.Extensions;
using Goblin.DataAccess;
using Goblin.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Serilog;
using Telegram.Bot;
using VkNet.Abstractions;
using VkNet.Exception;
using VkNet.Model.RequestParams;

namespace Goblin.BackgroundJobs.Jobs
{
    public class SendToChatTasks
    {
        private readonly TelegramBotClient _botClient;
        private readonly BotDbContext _context;
        private readonly IScheduleService _scheduleService;
        private readonly IVkApi _vkApi;
        private readonly IWeatherService _weatherService;
        private readonly MailingOptions _mailingOptions;
        private readonly ILogger _logger;

        public SendToChatTasks(IScheduleService scheduleService, IWeatherService weatherService, IVkApi vkApi,
                               TelegramBotClient botClient, IOptions<MailingOptions> mailingOptions, BotDbContext context)
        {
            _scheduleService = scheduleService;
            _weatherService = weatherService;
            _vkApi = vkApi;
            _botClient = botClient;
            _context = context;
            _mailingOptions = mailingOptions.Value;
            _logger = Log.ForContext<SendToChatTasks>();
        }

        public async Task Execute(long chatId, ConsumerType consumerType, CronType cronType, string city, int group, string text)
        {
            if(consumerType == ConsumerType.Vkontakte)
            {
                await Send(SendToVk);
            }
            else if(consumerType == ConsumerType.Telegram)
            {
                await Send(SendToTelegram);
            }

            async Task Send(Func<string, Task> func)
            {
                if(cronType.HasFlag(CronType.Schedule) && group != 0)
                {
                    await SendSchedule(chatId, group, func);
                }

                if(cronType.HasFlag(CronType.Weather) && !string.IsNullOrWhiteSpace(city))
                {
                    await SendWeather(chatId, city, func);
                }

                if(cronType.HasFlag(CronType.Text) && !string.IsNullOrWhiteSpace(text))
                {
                    await SendText(text, func);
                }
            }

            async Task SendToVk(string message)
            {
                try
                {
                    await _vkApi.Messages.SendWithRandomId(new MessagesSendParams
                    {
                        PeerId = chatId,
                        Message = message
                    });
                }
                catch(PermissionToPerformThisActionException)
                {
                    await RemoveJob(chatId);
                }
                catch(CannotSendToUserFirstlyException)
                {
                    await RemoveJob(chatId);
                }
            }

            async Task SendToTelegram(string message)
            {
                await _botClient.SendTextMessageAsync(chatId, message);
            }
        }

        private async Task RemoveJob(long chatId)
        {
            var job = _context.CronJobs.FirstOrDefaultAsync(x => x.ConsumerType == ConsumerType.Vkontakte &&
                                                                 x.ChatId == chatId);
            if(job != null)
            {
                _context.Remove(job);
                await _context.SaveChangesAsync();
            }
        }

        private async Task SendWeather(long id, string city, Func<string, Task> send)
        {
            _logger.Information("Отправка погоды в {0}", id);
            var result = await _weatherService.GetDailyWeather(city, DateTime.Today);

            await send(result.Message);
        }

        private async Task SendSchedule(long id, int group, Func<string, Task> send)
        {
            _logger.Information("Отправка расписания в {0}", id);
            var result = await _scheduleService.GetSchedule(group, DateTime.Now);
            if(!result.IsSuccessful && _mailingOptions.IsVacations)
            {
                return;
            }

            await send(result.Message);
        }

        private async Task SendText(string text, Func<string, Task> send)
        {
            await send(text);
        }
    }
}