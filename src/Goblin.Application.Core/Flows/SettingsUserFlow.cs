using Goblin.Application.Core.Extensions;
using Goblin.Domain;
using Goblin.Narfu.Abstractions;
using Goblin.OpenWeatherMap.Abstractions;

namespace Goblin.Application.Core.Flows;

public class SettingsUserFlow(INarfuApi narfuApi, IOpenWeatherMapApi openWeatherMapApi) : IUserFlow
{
    public string PayloadKey => PayloadType.Settings.GetEnumMemberValue();

    public FlowType Type => FlowType.Settings;

    public async Task<FlowExecutionResult> HandleAsync(UserFlowContext context, CancellationToken cancellationToken)
    {
        if (context.Message.ParsedPayload is not null &&
            context.Message.ParsedPayload.TryGetValue(PayloadKey, out var commandParam))
        {
            return HandleButton(context, commandParam);
        }

        return await HandleInput(context);
    }

    private static FlowExecutionResult HandleButton(UserFlowContext context, string commandParam)
    {
        if (string.IsNullOrWhiteSpace(commandParam))
        {
            return SettingsResult(context.User);
        }

        if (commandParam.Equals(SettingsFlowState.NarfuGroup.GetEnumMemberValue()))
        {
            return new FlowExecutionResult(
                FlowType.Settings,
                SettingsFlowState.NarfuGroup.GetEnumMemberValue(),
                true,
                "Отправьте номер группы САФУ. Номер должен содержать только цифры.",
                DefaultKeyboards.GetBackToSettingsKeyboard());
        }

        if (commandParam.Equals(SettingsFlowState.WeatherCity.GetEnumMemberValue()))
        {
            return new FlowExecutionResult(
                FlowType.Settings,
                SettingsFlowState.WeatherCity.GetEnumMemberValue(),
                true,
                "Отправьте название города для получения прогноза погоды.",
                DefaultKeyboards.GetBackToSettingsKeyboard());
        }

        if (commandParam.Equals(SettingsFlowState.Mailing.GetEnumMemberValue()))
        {
            return MailingResult(context.User, "Настройки рассылки:");
        }

        if (commandParam.Equals(SettingsFlowState.MailingSchedule.GetEnumMemberValue()))
        {
            if (context.User is { HasScheduleSubscription: false, NarfuGroup: null })
            {
                return MailingResult(context.User,
                    "Чтобы подписаться на ежедневную рассылку расписания, сначала настройте группу САФУ.",
                    false);
            }

            context.User.SetHasSchedule(!context.User.HasScheduleSubscription);
            var message = context.User.HasScheduleSubscription ? "Рассылка расписания включена." : "Рассылка расписания отключена.";
            return MailingResult(context.User, message);
        }

        if (commandParam.Equals(SettingsFlowState.MailingWeather.GetEnumMemberValue()))
        {
            if (!context.User.HasWeatherSubscription && string.IsNullOrWhiteSpace(context.User.WeatherCity))
            {
                return MailingResult(context.User,
                    "Чтобы подписаться на ежедневную рассылку погоду, сначала настройте город.",
                    false);
            }

            context.User.SetHasWeather(!context.User.HasWeatherSubscription);
            var message = context.User.HasWeatherSubscription ? "Рассылка погоды включена." : "Рассылка погоды отключена.";
            return MailingResult(context.User, message);
        }

        return SettingsResult(context.User, "Неизвестный пункт настроек.", false);
    }

    private async Task<FlowExecutionResult> HandleInput(UserFlowContext context)
    {
        var flowStep = context.User.Session.FlowStepType;
        if (flowStep == SettingsFlowState.NarfuGroup.GetEnumMemberValue())
        {
            if (!int.TryParse(context.Message.Text, out var realGroupNumber))
            {
                return new FlowExecutionResult(
                    FlowType.Settings,
                    SettingsFlowState.NarfuGroup.GetEnumMemberValue(),
                    false,
                    "Номер группы должен состоять только из цифр. Например, 351617.",
                    DefaultKeyboards.GetBackToSettingsKeyboard());
            }

            var group = narfuApi.Students.GetGroupByRealId(realGroupNumber);
            if (group is null)
            {
                return new FlowExecutionResult(
                    FlowType.Settings,
                    SettingsFlowState.NarfuGroup.GetEnumMemberValue(),
                    false,
                    "Группа с таким номером не найдена. Укажите другой номер.",
                    DefaultKeyboards.GetBackToSettingsKeyboard());
            }

            context.User.SetNarfuGroup(group.RealId);
            return SettingsResult(context.User, $"Группа {group.RealId} сохранена.");
        }

        if (flowStep == SettingsFlowState.WeatherCity.GetEnumMemberValue())
        {
            var city = context.Message.Text?.Trim();
            if (string.IsNullOrWhiteSpace(city) || !await openWeatherMapApi.IsCityExists(city))
            {
                return new FlowExecutionResult(
                    FlowType.Settings,
                    SettingsFlowState.WeatherCity.GetEnumMemberValue(),
                    false,
                    "Город не найден. Укажите другое название.",
                    DefaultKeyboards.GetBackToSettingsKeyboard());
            }

            context.User.SetCity(city);
            return SettingsResult(context.User, $"Город «{city}» сохранён.");
        }

        return SettingsResult(context.User);
    }

    private static FlowExecutionResult SettingsResult(
        BotUser user,
        string message = "Настройки:",
        bool isSuccessful = true)
    {
        return new FlowExecutionResult(
            FlowType.Settings,
            null,
            isSuccessful,
            message,
            DefaultKeyboards.GetSettingsKeyboard(user));
    }

    private static FlowExecutionResult MailingResult(
        BotUser user,
        string message,
        bool isSuccessful = true)
    {
        return new FlowExecutionResult(
            FlowType.Settings,
            null,
            isSuccessful,
            message,
            DefaultKeyboards.GetMailingSettingsKeyboard(user));
    }
}
