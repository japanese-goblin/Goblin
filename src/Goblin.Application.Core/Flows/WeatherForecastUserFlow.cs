using Goblin.Application.Core.Extensions;
using Goblin.Domain;

namespace Goblin.Application.Core.Flows;

public class WeatherForecastUserFlow(IWeatherService weatherService) : IUserFlow
{
    public string Name { get; }
    public string PayloadKey => PayloadType.ForecastWeather.GetEnumMemberValue();
    public FlowType Type => FlowType.ForecastWeather;

    public async Task<FlowExecutionResult> HandleAsync(UserFlowContext context, CancellationToken cancellationToken)
    {
        if(string.IsNullOrWhiteSpace(context.User.WeatherCity))
        {
            return new FlowExecutionResult(FlowType.MainMenu,
                                           null,
                                           false,
                                           "Отсутствует город для просмотра погоды - укажите его в настройках",
                                           null);
        }

        if(context.Message.ParsedPayload is null ||
           !context.Message.ParsedPayload.TryGetValue(PayloadKey, out var forecastDateInput))
        {
            return new FlowExecutionResult(FlowType.MainMenu,
                                           null,
                                           false, 
                                           "Воспользуйтесь клавиатурой для управления ботом:",
                                           DefaultKeyboards.GetMainMenuKeyboard());
        }

        if(!DateTime.TryParse(forecastDateInput, out var forecastDate))
        {
            
            return new FlowExecutionResult(FlowType.ForecastWeather,
                                           null,
                                           false, 
                                           "Указана некорректная дата",
                                           DefaultKeyboards.GetWeatherForecastKeyboard());
        }

        var getWeatherResponse = await weatherService.GetDailyWeather(context.User.WeatherCity, forecastDate);
        return new FlowExecutionResult(FlowType.ForecastWeather,
                                       null,
                                       getWeatherResponse.IsSuccessful,
                                       getWeatherResponse.Message,
                                       DefaultKeyboards.GetWeatherForecastKeyboard());
    }
}
