using System.Globalization;
using Goblin.DataAccess;
using Goblin.Domain;
using Microsoft.EntityFrameworkCore;

namespace Goblin.Application.Core.Commands.Text;

public class AddRemindCommand : ITextCommand
{
    public const int MaxRemindsCount = 8;

    public bool IsAdminCommand => false;

    public string[] Aliases => ["напомни"];

    private readonly BotDbContext _db;

    public AddRemindCommand(BotDbContext db)
    {
        _db = db;
    }

    public async Task<CommandExecutionResult> Execute(Message msg, BotUser user)
    {
        var param = string.Join(' ', msg.CommandParameters);
        var all = param.Split(' ', 3);

        var reminds = await _db.Reminds.Where(x => x.ChatId == user.Id && x.ConsumerType == user.ConsumerType)
                               .ToArrayAsync();

        if(!user.IsAdmin && reminds.Length == MaxRemindsCount)
        {
            return CommandExecutionResult.Failed($"Вы уже достигли максимального количества напоминаний ({MaxRemindsCount})");
        }

        if(all.Length != 3)
        {
            return CommandExecutionResult.Failed("Укажите дату, время и текст напоминания (11.11.2011 11:11 текст)");
        }

        if(all[0].Equals("завтра", StringComparison.OrdinalIgnoreCase))
        {
            var d = DateTimeOffset.Now.AddDays(1);
            all[0] = $"{d.Day}.{d.Month}.{d.Year}";
        }
        else if(all[0].Equals("сегодня", StringComparison.OrdinalIgnoreCase))
        {
            var d = DateTimeOffset.Now;
            all[0] = $"{d.Day}.{d.Month}.{d.Year}";
        }

        var isCorrectTime = ParseTime(all[0], all[1], out var dateTime);
        if(!isCorrectTime)
        {
            return CommandExecutionResult.Failed("Некорректная дата или время");
        }

        if(dateTime.ToUniversalTime() <= DateTimeOffset.UtcNow)
        {
            return CommandExecutionResult.Failed("Дата напоминания меньше текущей");
        }

        await AddRemind(user.Id, user.ConsumerType, all[2], dateTime);

        return CommandExecutionResult.Success($"Окей. {dateTime:f} напомню следующее:\n{all[2]}");
    }

    private async Task AddRemind(long chatId, ConsumerType consumerType, string remindText, DateTimeOffset dateTime)
    {
        await _db.Reminds.AddAsync(new Remind(chatId, remindText, dateTime, consumerType));
        await _db.SaveChangesAsync();
    }

    private static bool ParseTime(string date, string time, out DateTimeOffset dateTime)
    {
        var isCorrect = DateTimeOffset.TryParseExact($"{date} {time}",
        [
            "dd.MM.yyyy HH:mm", "d.MM.yyyy HH:mm",
            "dd.M.yyyy HH:mm", "d.M.yyyy HH:mm",
            "dd.MM.yyyy H:mm", "d.MM.yyyy H:mm",
            "dd.M.yyyy H:mm", "d.M.yyyy H:mm",
            "dd.MM.yyyy HH:m", "d.MM.yyyy HH:m",
            "dd.M.yyyy HH:m", "d.M.yyyy HH:m",
            "dd.MM.yyyy H:m", "d.MM.yyyy H:m",
            "dd.M.yyyy H:m", "d.M.yyyy H:m"
        ], CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out dateTime);

        return isCorrect;
    }
}