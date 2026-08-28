using Goblin.Application.Core.Flows;

namespace Goblin.Application.Core;

public static class DefaultKeyboards
{
    private const string DefaultDateFormat = "yyyy-MM-dd";

    public static CoreKeyboard GetMainMenuKeyboard()
    {
        var payloadType = PayloadType.Menu.GetEnumMemberValue();
        var kb = new CoreKeyboard
        {
            IsOneTime = false,
            IsInline = false
        };

        kb.AddButton("Расписание",
                     CoreKeyboardButtonColor.Primary,
                     payloadType,
                     MainMenuFlowState.Schedule.GetEnumMemberValue())
          .AddButton("Экзамены",
                     CoreKeyboardButtonColor.Primary,
                     payloadType,
                     MainMenuFlowState.Exams.GetEnumMemberValue())
          .AddLine()
          .AddButton("Погода сейчас",
                     CoreKeyboardButtonColor.Primary,
                     payloadType,
                     MainMenuFlowState.CurrentWeather.GetEnumMemberValue())
          .AddButton("Прогноз по дням",
                     CoreKeyboardButtonColor.Primary,
                     payloadType,
                     MainMenuFlowState.ForecastWeather.GetEnumMemberValue())
          .AddLine()
          .AddButton("Настройки",
                     CoreKeyboardButtonColor.Primary,
                     payloadType,
                     MainMenuFlowState.Settings.GetEnumMemberValue())
          ;
        return kb;
    }

    public static CoreKeyboard GetInitializationKeyboard(BotUser user)
    {
        var payloadType = PayloadType.Init.GetEnumMemberValue();
        var hasNarfuGroup = user.NarfuGroup.HasValue;
        var hasWeatherCity = !string.IsNullOrWhiteSpace(user.WeatherCity);

        var keyboard = new CoreKeyboard
        {
            IsInline = true
        };
        keyboard.AddButton(hasNarfuGroup ? "📅 Изменить группу САФУ" : "📅 Настроить группу САФУ",
                           CoreKeyboardButtonColor.Primary,
                           payloadType,
                           InitialFlowState.SettingNarfuGroup.GetEnumMemberValue())
                .AddLine()
                .AddButton(hasWeatherCity ? "⛅ Изменить город" : "⛅ Настроить город",
                           CoreKeyboardButtonColor.Primary,
                           payloadType,
                           InitialFlowState.SettingWeather.GetEnumMemberValue())
                .AddLine()
                .AddButton(hasNarfuGroup || hasWeatherCity ? "Далее ✅" : "Пропустить 👉",
                           CoreKeyboardButtonColor.Default,
                           payloadType,
                           InitialFlowState.Continue.GetEnumMemberValue());

        return keyboard;
    }

    public static CoreKeyboard GetScheduleKeyboard()
    {
        var payloadType = PayloadType.Schedule.GetEnumMemberValue();
        var keyboard = new CoreKeyboard
        {
            IsInline = true
        };

        var date = DateTime.Now;
        keyboard.AddButton($"На сегодня ({date:dd.MM - dddd})",
                           CoreKeyboardButtonColor.Primary,
                           payloadType, 
                           date.ToString(DefaultDateFormat));

        date = date.AddDays(1);
        if(date.DayOfWeek != DayOfWeek.Sunday)
        {
            keyboard.AddButton($"На завтра ({date:dd.MM - dddd})",
                               CoreKeyboardButtonColor.Primary,
                               payloadType,
                               date.ToString(DefaultDateFormat));
            keyboard.AddLine();
        }

        for(var i = 1; i < 7; i++)
        {
            date = date.AddDays(1);
            if(date.DayOfWeek == DayOfWeek.Sunday)
            {
                continue;
            }

            keyboard.AddButton($"На {date:dd.MM (dddd)}",
                               CoreKeyboardButtonColor.Primary,
                               payloadType,
                               date.ToString(DefaultDateFormat));
            if(i % 2 == 0)
            {
                keyboard.AddLine();
            }
        }

        return keyboard;
    }
    
    public static CoreKeyboard GetWeatherForecastKeyboard()
    {
        var payloadType = PayloadType.ForecastWeather.GetEnumMemberValue();

        var keyboard = new CoreKeyboard
        {
            IsInline = true
        };

        var date = DateTime.Now;
        keyboard.AddButton($"На сегодня ({date:dd.MM - dddd})",
                           CoreKeyboardButtonColor.Primary,
                           payloadType, 
                           date.ToString(DefaultDateFormat));

        date = date.AddDays(1);
        keyboard.AddButton($"На завтра ({date:dd.MM - dddd})",
                           CoreKeyboardButtonColor.Primary,
                           payloadType,
                           date.ToString(DefaultDateFormat));

        return keyboard;
    }

    public static CoreKeyboard GetSettingsKeyboard(BotUser user)
    {
        var payloadType = PayloadType.Settings.GetEnumMemberValue();
        var groupText = user.NarfuGroup.HasValue
            ? $"📅 Группа САФУ: {user.NarfuGroup}"
            : "📅 Настроить группу САФУ";
        var cityText = string.IsNullOrWhiteSpace(user.WeatherCity)
            ? "⛅ Настроить город для погоды"
            : $"⛅ Город: {user.WeatherCity}";

        var keyboard = new CoreKeyboard
        {
            IsInline = true
        };
        keyboard.AddButton(groupText,
                           CoreKeyboardButtonColor.Primary,
                           payloadType,
                           SettingsFlowState.NarfuGroup.GetEnumMemberValue())
                .AddLine()
                .AddButton(cityText,
                           CoreKeyboardButtonColor.Primary,
                           payloadType,
                           SettingsFlowState.WeatherCity.GetEnumMemberValue())
                .AddLine()
                .AddButton("🔔 Рассылка",
                           CoreKeyboardButtonColor.Primary,
                           payloadType,
                           SettingsFlowState.Mailing.GetEnumMemberValue());

        return keyboard;
    }

    public static CoreKeyboard GetMailingSettingsKeyboard(BotUser user)
    {
        var payloadType = PayloadType.Settings.GetEnumMemberValue();
        var scheduleText = user.HasScheduleSubscription
            ? "❌ Отключить рассылку расписания"
            : "✅ Включить рассылку расписания";
        var weatherText = user.HasWeatherSubscription
            ? "❌ Отключить рассылку погоды"
            : "✅ Включить рассылку погоды";

        var keyboard = new CoreKeyboard
        {
            IsInline = true
        };
        keyboard.AddButton(scheduleText,
                           user.HasScheduleSubscription
                               ? CoreKeyboardButtonColor.Negative
                               : CoreKeyboardButtonColor.Positive,
                           payloadType,
                           SettingsFlowState.MailingSchedule.GetEnumMemberValue())
                .AddLine()
                .AddButton(weatherText,
                           user.HasWeatherSubscription
                               ? CoreKeyboardButtonColor.Negative
                               : CoreKeyboardButtonColor.Positive,
                           payloadType,
                           SettingsFlowState.MailingWeather.GetEnumMemberValue())
                .AddLine()
                .AddButton("Вернуться в настройки",
                           CoreKeyboardButtonColor.Default,
                           payloadType,
                           string.Empty);

        return keyboard;
    }

    public static CoreKeyboard GetBackToSettingsKeyboard()
    {
        var keyboard = new CoreKeyboard
        {
            IsInline = true
        };
        keyboard.AddButton("Вернуться в настройки",
                           CoreKeyboardButtonColor.Default,
                           PayloadType.Settings.GetEnumMemberValue(),
                           string.Empty);
        return keyboard;
    }
}
