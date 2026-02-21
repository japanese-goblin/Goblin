namespace Goblin.Application.Core.Commands.Merged;

public class WeatherNowCommand(IWeatherService weatherService) : IKeyboardCommand, ITextCommand
{
    public string Trigger => "weatherNow";

    public bool IsAdminCommand => false;

    public string[] Aliases => ["погода"];

    public async Task<CommandExecutionResult> Execute(Message msg, BotUser user)
    {
        if(!string.IsNullOrWhiteSpace(msg.Payload))
        {
            return await ExecutePayload(user);
        }

        return await ExecuteText(msg, user);
    }

    private async Task<CommandExecutionResult> ExecuteText(Message msg, BotUser user)
    {
        var city = msg.CommandParameters.FirstOrDefault();

        if(string.IsNullOrWhiteSpace(user.WeatherCity) && string.IsNullOrWhiteSpace(city))
        {
            return CommandExecutionResult.Failed(DefaultErrors.CityNotSet);
        }

        if(!string.IsNullOrWhiteSpace(city))
        {
            return await weatherService.GetCurrentWeather(city);
        }

        return await weatherService.GetCurrentWeather(user.WeatherCity);
    }

    private async Task<CommandExecutionResult> ExecutePayload(BotUser user)
    {
        if(string.IsNullOrWhiteSpace(user.WeatherCity))
        {
            return CommandExecutionResult.Failed(DefaultErrors.CityNotSet);
        }

        return await weatherService.GetCurrentWeather(user.WeatherCity);
    }
}