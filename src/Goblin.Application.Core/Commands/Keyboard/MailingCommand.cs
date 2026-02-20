using Goblin.DataAccess;

namespace Goblin.Application.Core.Commands.Keyboard;

public class MailingCommand(BotDbContext db) : IKeyboardCommand
{
    private const string Success = "Успешно.";

    public string Trigger => "mailing";

    public async Task<CommandExecutionResult> Execute(Message msg, BotUser user)
    {
        user = db.Entry(user).Entity;
        var choose = msg.ParsedPayload[Trigger];
        var isSchedule = user.HasScheduleSubscription;
        var isWeather = user.HasWeatherSubscription;
        if(choose.Equals("weather", StringComparison.OrdinalIgnoreCase))
        {
            return await SetWeatherMailing(user, isWeather);
        }

        if(choose.Equals("schedule", StringComparison.OrdinalIgnoreCase))
        {
            return await SetScheduleMailing(user, isSchedule);
        }

        return CommandExecutionResult.Failed("Действие не найдено");
    }

    private async Task<CommandExecutionResult> SetScheduleMailing(BotUser user, bool isSchedule)
    {
        if(user.NarfuGroup == 0)
        {
            return CommandExecutionResult.Failed(DefaultErrors.GroupNotSet);
        }

        user.SetHasSchedule(!isSchedule);
        await db.SaveChangesAsync();
        return CommandExecutionResult.Success(Success, DefaultKeyboards.GetMailingKeyboard(user));
    }

    private async Task<CommandExecutionResult> SetWeatherMailing(BotUser user, bool isWeather)
    {
        if(string.IsNullOrWhiteSpace(user.WeatherCity))
        {
            return CommandExecutionResult.Failed(DefaultErrors.CityNotSet);
        }

        user.SetHasWeather(!isWeather);
        await db.SaveChangesAsync();
        return CommandExecutionResult.Success(Success, DefaultKeyboards.GetMailingKeyboard(user));
    }
}