using Goblin.Domain;
using Goblin.Narfu.Abstractions;
using Goblin.OpenWeatherMap.Abstractions;

namespace Goblin.Application.Core.Flows;

public class InitialUserFlow(INarfuApi narfuApi, IOpenWeatherMapApi openWeatherMapApi) : IUserFlow
{
    public string Name => "Первый запуск";
    public string PayloadKey => PayloadType.Init.GetEnumMemberValue();
    public FlowType Type => FlowType.Start;

    public async Task<FlowExecutionResult> HandleAsync(UserFlowContext context, CancellationToken cancellationToken)
    {
        // проверяем состояние сессии
        var flowStepType = context.User.Session.FlowStepType;
        if (!string.IsNullOrWhiteSpace(flowStepType))
        {
            var userInput = context.Message.Text.ToLowerInvariant();
            // если состояние - установка группа
            if (flowStepType == InitialFlowState.SettingNarfuGroup.GetEnumMemberValue())
            {
                // проверяем, что прислали номер группы
                if (!int.TryParse(userInput, out var realGroupNumber))
                {
                    return new FlowExecutionResult(FlowType.Start,
                        InitialFlowState.SettingNarfuGroup.GetEnumMemberValue(),
                        false,
                        "Номер группы должен состоять только из цифр. Например, 351617",
                        null);
                }

                // проверяем существование группы
                var narfuGroup = narfuApi.Students.GetGroupByRealId(realGroupNumber);
                if (narfuGroup is null)
                {
                    return
                        new FlowExecutionResult(FlowType.Start,
                            InitialFlowState.SettingNarfuGroup.GetEnumMemberValue(),
                            false,
                            "Группа с таким номером не найдена. Укажите другой номер или пропустите шаг",
                            null);
                }

                // устанавливаем группу
                context.User.SetNarfuGroup(narfuGroup.RealId);
                return new FlowExecutionResult(FlowType.Start,
                    null,
                    false,
                    "Группа успешно установлена!",
                    DefaultKeyboards.GetInitialKeyboard());
            }

            // если состояние - установка города
            if (flowStepType == InitialFlowState.SettingWeather.GetEnumMemberValue())
            {
                var isCityExists = await openWeatherMapApi.IsCityExists(userInput);
                if (!isCityExists)
                {
                    return new FlowExecutionResult(FlowType.Start,
                        InitialFlowState.SettingWeather.GetEnumMemberValue(),
                        false,
                        "Указанный город не найден. Укажите другой или пропустите шаг",
                        null);
                }

                context.User.SetCity(userInput);
                return new FlowExecutionResult(
                    FlowType.Start,
                    null,
                    true,
                    "Город успешно установлен!",
                    DefaultKeyboards.GetInitialKeyboard());
            }
        }

        // начальное состояние - человек впервые воспользовался ботом
        if (string.IsNullOrEmpty(context.Message.Payload))
        {
            var response = new FlowExecutionResult(
                FlowType.Start,
                null,
                true,
                """
                Добро пожаловать!
                Перед началом работы Вы можете сохранить номер группы для получения расписания САФУ,
                а также город - для ежедневного получения прогноза погоды.
                Для продолжения воспользуйтесь кнопками ниже:
                """,
                DefaultKeyboards.GetInitialKeyboard()
            );
            return response;
        }

        // пользователь нажал на одну из кнопок клавиатуры
        if (context.Message.ParsedPayload.TryGetValue(PayloadKey, out var commandParam))
        {
            // установка группы САФУ
            if (commandParam.Equals(InitialFlowState.SettingNarfuGroup.GetEnumMemberValue()))
            {
                var response = new FlowExecutionResult(FlowType.Start,
                    InitialFlowState.SettingNarfuGroup.GetEnumMemberValue(),
                    true,
                    "Отправь номер группы",
                    null);
                return response;
            }

            // установка города для погоды
            if (commandParam.Equals(InitialFlowState.SettingWeather.GetEnumMemberValue()))
            {
                var response = new FlowExecutionResult(FlowType.Start,
                    InitialFlowState.SettingWeather.GetEnumMemberValue(),
                    true,
                    "Отправь название города",
                    null);
                return response;
            }

            // переход в главное меню
            if (commandParam.Equals(InitialFlowState.Skip.GetEnumMemberValue()))
            {
                var response = new FlowExecutionResult(
                    FlowType.MainMenu,
                    null,
                    true,
                    "Вы в главном меню, тут потом основная клавиатура будет",
                    null);
                return response;
            }
        }

        return new FlowExecutionResult(
            FlowType.Start,
            null,
            false,
            "что-то пошло не так...",
            null);
    }
}