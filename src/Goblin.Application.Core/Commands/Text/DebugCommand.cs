using System.Diagnostics;
using System.Text;
using Goblin.DataAccess;
using Goblin.Domain;

namespace Goblin.Application.Core.Commands.Text;

public class DebugCommand : ITextCommand
{
    public bool IsAdminCommand => true;

    public string[] Aliases => ["дебуг", "дебаг"];

    private readonly BotDbContext _db;

    public DebugCommand(BotDbContext db)
    {
        _db = db;
    }

    public Task<CommandExecutionResult> Execute(Message msg, BotUser user)
    {
        var strBuilder = new StringBuilder();

        var birthday = new DateTime(2017, 4, 29, 19, 42, 0);
        var dis = DateTime.Now - birthday;

        var startTime = Process.GetCurrentProcess().StartTime;
        var uptime = DateTime.Now - startTime;

        var consumerTypeCounts = _db.BotUsers.AsEnumerable()
                                    .GroupBy(x => x.ConsumerType)
                                    .ToDictionary(x => x.Key, x => x.Count());

        strBuilder.Append($"Время старта: {startTime:F}").AppendLine()
                  .Append($"Я работаю уже {uptime.Hours} часов {uptime.Minutes} минут")
                  .AppendLine()
                  .Append($"Мне уже {dis.Days} дней ({birthday:dd.MM.yyyy})")
                  .AppendLine().AppendLine()
                  .Append($"Всего пользователей {consumerTypeCounts.Sum(x => x.Value)}, из них:")
                  .AppendLine();
        foreach(var consumerTypeCount in consumerTypeCounts)
        {
            strBuilder.Append($"* {consumerTypeCount.Key} - {consumerTypeCount.Value}")
                      .AppendLine();
        }

        var subscriptions = _db.BotUsers.AsEnumerable().GroupBy(x => x.ConsumerType)
                               .Select(x => new GroupUsersResponse
                               {
                                   ConsumerType = x.Key,
                                   ScheduleSubscriptions = x.Count(u => u.HasScheduleSubscription),
                                   WeatherSubscriptions = x.Count(u => u.HasWeatherSubscription)
                               }).ToArray();

        strBuilder.AppendLine()
                  .Append($"Подписки - {subscriptions.Sum(x => x.WeatherSubscriptions)} погода, {subscriptions.Sum(x => x.ScheduleSubscriptions)} расписание. Из них:")
                  .AppendLine();
        foreach(var subscription in subscriptions)
        {
            strBuilder
                    .Append($"* {subscription.ConsumerType} - {subscription.WeatherSubscriptions} погода, {subscription.ScheduleSubscriptions} расписание")
                    .AppendLine();
        }

        return Task.FromResult(CommandExecutionResult.Success(strBuilder.ToString()));
    }

    private class GroupUsersResponse
    {
        public ConsumerType ConsumerType { get; set; }
        public int WeatherSubscriptions { get; set; }
        public int ScheduleSubscriptions { get; set; }
    }
}