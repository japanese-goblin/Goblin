using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Goblin.Application.Core.Abstractions;
using Goblin.Application.Core.Models;
using Goblin.Application.Core.Results.Success;
using Goblin.DataAccess;
using Goblin.Domain;
using Goblin.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Goblin.Application.Core.Commands.Text;

public class DebugCommand : ITextCommand
{
    public bool IsAdminCommand => true;
    public string[] Aliases => new[] { "дебуг", "дебаг" };

    private readonly BotDbContext _db;

    public DebugCommand(BotDbContext db)
    {
        _db = db;
    }

    public async Task<IResult> Execute(Message msg, BotUser user)
    {
        var strBuilder = new StringBuilder();

        var birthday = new DateTime(2017, 4, 29, 19, 42, 0);
        var dis = DateTime.Now - birthday;

        var startTime = Process.GetCurrentProcess().StartTime;
        var uptime = DateTime.Now - startTime;

        var consumerTypeCounts = await _db.BotUsers.GroupBy(x => x.ConsumerType)
                                          .ToDictionaryAsync(x => x.Key, x => x.Count());

        strBuilder.AppendFormat("Время старта: {0:F}", startTime).AppendLine()
                  .AppendFormat("Я работаю уже {0} часов {1} минут", uptime.Hours, uptime.Minutes)
                  .AppendLine()
                  .AppendFormat("Мне уже {0} дней ({1:dd.MM.yyyy})", dis.Days, birthday)
                  .AppendLine().AppendLine()
                  .AppendFormat("Всего пользователей {0}, из них:", consumerTypeCounts.Sum(x => x.Value));
        foreach(var consumerTypeCount in consumerTypeCounts)
        {
            strBuilder.AppendFormat("\t{0} - {1}", consumerTypeCount.Key, consumerTypeCount.Value)
                      .AppendLine();
        }

        var subscriptions = await _db.BotUsers.GroupBy(x => x.ConsumerType)
                         .Select(x => new GroupUsersResponse
                         {
                             ConsumerType = x.Key,
                             ScheduleSubscriptions = x.Count(u => u.HasScheduleSubscription),
                             WeatherSubscriptions = x.Count(u => u.HasWeatherSubscription)
                         }).ToArrayAsync();

        strBuilder.AppendLine("Подписки:");
        foreach(var subscription in subscriptions)
        {
            strBuilder.AppendFormat("\t{0} - {1} погода, {2} расписание",
                                    subscription.ConsumerType, subscription.WeatherSubscriptions,
                                    subscription.ScheduleSubscriptions)
                      .AppendLine();
        }
        
        strBuilder.AppendFormat("Кодировка: {0}", Encoding.Default.BodyName);

        return new SuccessfulResult
        {
            Message = strBuilder.ToString()
        };
    }

    private class GroupUsersResponse
    {
        public ConsumerType ConsumerType { get; set; }
        public int WeatherSubscriptions { get; set; }
        public int ScheduleSubscriptions { get; set; }
    }
}