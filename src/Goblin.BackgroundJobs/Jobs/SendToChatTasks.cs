using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Goblin.Application.Core;
using Goblin.Application.Core.Abstractions;
using Goblin.Application.Core.Options;
using Goblin.DataAccess;
using Goblin.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Serilog;

namespace Goblin.BackgroundJobs.Jobs;

public class SendToChatTasks
{
    private readonly BotDbContext _context;
    private readonly IScheduleService _scheduleService;
    private readonly IWeatherService _weatherService;
    private readonly IEnumerable<ISender> _senders;
    private readonly MailingOptions _mailingOptions;
    private readonly ILogger _logger;

    public SendToChatTasks(IScheduleService scheduleService, IWeatherService weatherService,
                           IEnumerable<ISender> senders,
                           IOptions<MailingOptions> mailingOptions,
                           BotDbContext context)
    {
        _scheduleService = scheduleService;
        _weatherService = weatherService;
        _senders = senders;
        _context = context;
        _mailingOptions = mailingOptions.Value;
        _logger = Log.ForContext<SendToChatTasks>();
    }

    public async Task Execute(long chatId, ConsumerType consumerType, CronType cronType, string city, int group, string text)
    {
        var sender = _senders.FirstOrDefault(x => x.ConsumerType == consumerType);
        await Send(responseText => sender.Send(chatId, responseText));

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
        if(DateTime.Today.DayOfWeek == DayOfWeek.Sunday || _mailingOptions.IsVacations)
        {
            return;
        }

        _logger.Information("Отправка расписания в {0}", id);
        var result = await _scheduleService.GetSchedule(group, DateTime.Now);

        await send(result.Message);
    }

    private async Task SendText(string text, Func<string, Task> send)
    {
        await send(text);
    }
}