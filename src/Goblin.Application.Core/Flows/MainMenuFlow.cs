using Goblin.Application.Core.Extensions;
using Goblin.Domain;
using Goblin.Narfu.Abstractions;

namespace Goblin.Application.Core.Flows;

public class MainMenuUserFlow(IWeatherService weatherService, IScheduleService scheduleService) : IUserFlow
{
    public string PayloadKey => PayloadType.Menu.GetEnumMemberValue();

    public FlowType Type => FlowType.MainMenu;

    public async Task<FlowExecutionResult> HandleAsync(UserFlowContext context, CancellationToken cancellationToken)
    {
        if (context.Message.ParsedPayload is not null &&
            context.Message.ParsedPayload.TryGetValue(PayloadKey, out var commandParam))
        {
            if (string.IsNullOrWhiteSpace(commandParam))
            {
                return new FlowExecutionResult(
                    FlowType.MainMenu,
                    null,
                    true,
                    "Воспользуйтесь клавиатурой для управления ботом:",
                    DefaultKeyboards.GetMainMenuKeyboard());
            }

            if (commandParam.Equals(MainMenuFlowState.Schedule.GetEnumMemberValue()))
            {
                return new FlowExecutionResult(
                    FlowType.Schedule,
                    null,
                    true,
                    "Выберите день для получения расписания",
                    DefaultKeyboards.GetScheduleKeyboard());
            }

            if (commandParam.Equals(MainMenuFlowState.Exams.GetEnumMemberValue()))
            {
                if (!context.User.NarfuGroup.HasValue)
                {
                    return new FlowExecutionResult(
                        FlowType.MainMenu,
                        null,
                        false,
                        "Отсутствует группа САФУ - укажите её в настройках",
                        null);
                }

                var exams = await scheduleService.GetExams(context.User.NarfuGroup.Value, cancellationToken);
                return new FlowExecutionResult(FlowType.MainMenu, null, true, exams.Message, null);
            }

            // получение погоды
            if (commandParam.Equals(MainMenuFlowState.CurrentWeather.GetEnumMemberValue()))
            {
                if (string.IsNullOrWhiteSpace(context.User.WeatherCity))
                {
                    return new FlowExecutionResult(
                        FlowType.MainMenu,
                        null,
                        false,
                        "Отсутствует город для просмотра погоды - укажите его в настройках",
                        null);
                }

                var getWeatherResult = await weatherService.GetCurrentWeather(context.User.WeatherCity, cancellationToken);
                return new FlowExecutionResult(FlowType.MainMenu, null, true, getWeatherResult.Message, null);
            }

            if (commandParam.Equals(MainMenuFlowState.ForecastWeather.GetEnumMemberValue()))
            {
                return new FlowExecutionResult(
                    FlowType.ForecastWeather,
                    null,
                    true,
                    "Выберите день для получения погоды",
                    DefaultKeyboards.GetWeatherForecastKeyboard());
            }

            if (commandParam.Equals(MainMenuFlowState.Settings.GetEnumMemberValue()))
            {
                return new FlowExecutionResult(
                    FlowType.Settings,
                    null,
                    true,
                    "Настройки:",
                    DefaultKeyboards.GetSettingsKeyboard(context.User));
            }
        }

        return new FlowExecutionResult(
            FlowType.MainMenu,
            null,
            false,
            "Воспользуйтесь клавиатурой для управления ботом:",
            DefaultKeyboards.GetMainMenuKeyboard());
    }
}
