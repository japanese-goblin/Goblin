using Goblin.Application.Core.Extensions;
using Goblin.Domain;
using Goblin.Narfu.Abstractions;
using Goblin.OpenWeatherMap.Abstractions;

namespace Goblin.Application.Core.Flows;

public class InitialUserFlow(INarfuApi narfuApi, IOpenWeatherMapApi openWeatherMapApi) : IUserFlow
{
    public string PayloadKey => PayloadType.Init.GetEnumMemberValue();

    public FlowType Type => FlowType.Start;

    public async Task<FlowExecutionResult> HandleAsync(UserFlowContext context, CancellationToken cancellationToken)
    {
        // пользователь нажал на одну из кнопок клавиатуры
        if (context.Message.ParsedPayload is not null &&
            context.Message.ParsedPayload.TryGetValue(PayloadKey, out var commandParam))
        {
            // установка группы САФУ
            if (commandParam.Equals(InitialFlowState.SettingNarfuGroup.GetEnumMemberValue()))
            {
                var response = new FlowExecutionResult(FlowType.Start,
                    InitialFlowState.SettingNarfuGroup.GetEnumMemberValue(),
                    true,
                    "Отправь номер группы, для которой будешь получать расписание 📅.\n" +
                    "❗Номер группы должен содержать ТОЛЬКО цифры!",
                    GetSkipKeyboard());
                return response;
            }

            // установка города для погоды
            if (commandParam.Equals(InitialFlowState.SettingWeather.GetEnumMemberValue()))
            {
                var response = new FlowExecutionResult(FlowType.Start,
                    InitialFlowState.SettingWeather.GetEnumMemberValue(),
                    true,
                    "Отправь название города, для которого будешь получать прогноз погоды ⛅",
                    GetSkipKeyboard());
                return response;
            }

            // пропуск установки какого-либо параметра
            if (commandParam.Equals(InitialFlowState.Skip.GetEnumMemberValue()))
            {
                var response = new FlowExecutionResult(FlowType.Start,
                    null,
                    true,
                    "🐸",
                    DefaultKeyboards.GetInitializationKeyboard(context.User));
                return response;
            }

            // переход в главное меню
            if (commandParam.Equals(InitialFlowState.Continue.GetEnumMemberValue()))
            {
                var response = new FlowExecutionResult(
                    FlowType.MainMenu,
                    null,
                    true,
                    "Настройки успешно сохранены! Теперь Вы можете пользоваться ботом при помощи кнопок меню:",
                    DefaultKeyboards.GetMainMenuKeyboard());
                return response;
            }
        }

        // проверяем состояние сессии
        var flowStepType = context.User.Session.FlowStepType;
        if (!string.IsNullOrWhiteSpace(flowStepType))
        {
            // если состояние - установка группа
            if (flowStepType == InitialFlowState.SettingNarfuGroup.GetEnumMemberValue())
            {
                var userInput = context.Message.Text?.ToLowerInvariant();
                // проверяем, что прислали номер группы
                if (!int.TryParse(userInput, out var realGroupNumber))
                {
                    return new FlowExecutionResult(FlowType.Start,
                        InitialFlowState.SettingNarfuGroup.GetEnumMemberValue(),
                        false,
                        "Номер группы должен состоять только из цифр. Например, 351617",
                        GetSkipKeyboard());
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
                            GetSkipKeyboard());
                }

                // устанавливаем группу
                context.User.SetNarfuGroup(narfuGroup.RealId);
                return new FlowExecutionResult(FlowType.Start,
                    null,
                    false,
                    "Группа успешно установлена!",
                    DefaultKeyboards.GetInitializationKeyboard(context.User));
            }

            // если состояние - установка города
            if (flowStepType == InitialFlowState.SettingWeather.GetEnumMemberValue())
            {
                var userInput = context.Message.Text?.ToLowerInvariant();
                var isCityExists = await openWeatherMapApi.IsCityExists(userInput);
                if (!isCityExists)
                {
                    return new FlowExecutionResult(FlowType.Start,
                        InitialFlowState.SettingWeather.GetEnumMemberValue(),
                        false,
                        "Указанный город не найден. Укажите другой или пропустите шаг",
                        GetSkipKeyboard());
                }

                context.User.SetCity(userInput);
                return new FlowExecutionResult(
                    FlowType.Start,
                    null,
                    true,
                    "Город успешно установлен!",
                    DefaultKeyboards.GetInitializationKeyboard(context.User));
            }
        }
        else
        {
            // если у него пустой шаг действия
            var response = new FlowExecutionResult(
                FlowType.Start,
                null,
                true,
                """
                Добро пожаловать! 🐸
                Перед началом работы Вы можете сохранить номер группы для получения расписания САФУ, а также город для получения прогноза погоды.
                Для продолжения воспользуйтесь кнопками ниже:
                """,
                DefaultKeyboards.GetInitializationKeyboard(context.User)
            );
            return response;
        }

        return new FlowExecutionResult(
            FlowType.Start,
            null,
            false,
            "👺 что-то пошло не так...",
            null);
    }

    private CoreKeyboard GetSkipKeyboard()
    {
        var kb = new CoreKeyboard { IsInline = true };
        kb.AddButton("Пропустить",
            CoreKeyboardButtonColor.Default,
            PayloadKey,
            InitialFlowState.Skip.GetEnumMemberValue());
        return kb;
    }
}
