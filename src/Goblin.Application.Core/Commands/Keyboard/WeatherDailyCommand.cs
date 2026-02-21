namespace Goblin.Application.Core.Commands.Keyboard;

public class WeatherDailyCommand(IWeatherService weatherService) : IKeyboardCommand
{
    public string Trigger => "weatherDaily";

    public async Task<CommandExecutionResult> Execute(Message msg, BotUser user)
    {
        if(string.IsNullOrWhiteSpace(user.WeatherCity))
        {
            return
                    CommandExecutionResult
                            .Failed("Для получения погоды установите город (нужно написать следующее - установить город Москва).");
        }

        var dict = msg.ParsedPayload;
        var isExists = dict.TryGetValue(Trigger, out var day);
        if(!isExists)
        {
            return CommandExecutionResult.Failed("Невозможно получить значение даты");
        }

        var isCorrectDate = DateTime.TryParse(day, out var dateTime);
        if(!isCorrectDate)
        {
            return CommandExecutionResult.Failed("Некорректное значение даты");
        }

        var result = await weatherService.GetDailyWeather(user.WeatherCity, dateTime);
        result.Keyboard = DefaultKeyboards.GetDailyWeatherKeyboard();
        return result;
    }
}