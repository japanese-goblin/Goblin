namespace Goblin.Application.Core.Commands.Merged;

public class WeatherDailyKeyboardCommand : IKeyboardCommand, ITextCommand
{
    public string Trigger => "weatherDailyKeyboard";

    public bool IsAdminCommand => false;

    public string[] Aliases => ["ежедневная"]; //TODO: ?

    public Task<CommandExecutionResult> Execute(Message msg, BotUser user)
    {
        if(string.IsNullOrWhiteSpace(user.WeatherCity))
        {
            return Task.FromResult(CommandExecutionResult.Failed(DefaultErrors.CityNotSet));
        }

        return Task.FromResult(CommandExecutionResult.Success("Выберите день для получения погоды:",
                                                              DefaultKeyboards.GetDailyWeatherKeyboard()));
    }
}