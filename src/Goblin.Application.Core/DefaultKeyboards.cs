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
    
    
    public static CoreKeyboard GetScheduleKeyboardV2()
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


    public static CoreKeyboard GetDefaultKeyboard()
    {
        var kb = new CoreKeyboard(false);
        kb.AddButton("Расписание", CoreKeyboardButtonColor.Primary, "scheduleKeyboard", string.Empty)
          .AddButton("Экзамены", CoreKeyboardButtonColor.Primary, "exams", string.Empty)
          .AddLine()
          .AddButton("Погода", CoreKeyboardButtonColor.Primary, "weatherNow", string.Empty)
          .AddButton("Ежедневная погода", CoreKeyboardButtonColor.Primary, "weatherDailyKeyboard", string.Empty)
          .AddLine()
          .AddButton("Рассылка", CoreKeyboardButtonColor.Primary, "mailingKeyboard", string.Empty)
          .AddButton("Напоминания", CoreKeyboardButtonColor.Default, "reminds", string.Empty)
          .AddButton("Справка", CoreKeyboardButtonColor.Primary, "help", string.Empty);

        return kb;
    }

    public static CoreKeyboard GetMailingKeyboard(BotUser user)
    {
        const string mailingKey = "mailing";
        var isSchedule = user.HasScheduleSubscription;
        var isWeather = user.HasWeatherSubscription;

        var scheduleColor = isSchedule ? CoreKeyboardButtonColor.Negative : CoreKeyboardButtonColor.Positive;
        var weatherColor = isWeather ? CoreKeyboardButtonColor.Negative : CoreKeyboardButtonColor.Positive;

        var scheduleText = isSchedule ? "❌Отписаться от рассылки расписания" : "✔Подписаться на рассылку расписания";
        var weatherText = isWeather ? "❌Отписаться от рассылки погоды" : "✔Подписаться на рассылку погоды";

        var kb = new CoreKeyboard
        {
            IsInline = true
        };
        kb.AddButton(scheduleText, scheduleColor, mailingKey, "schedule")
          .AddLine()
          .AddButton(weatherText, weatherColor, mailingKey, "weather")
          .AddReturnToMenuButton();

        return kb;
    }

    public static CoreKeyboard GetScheduleKeyboard()
    {
        var date = DateTime.Now;

        var keyboard = new CoreKeyboard
        {
            IsInline = true
        };
        keyboard.AddButton($"На сегодня ({date:dd.MM - dddd})",
                           CoreKeyboardButtonColor.Primary,
                           "schedule", 
                           date.ToString(DefaultDateFormat));
        keyboard.AddLine();

        date = date.AddDays(1);
        if(date.DayOfWeek != DayOfWeek.Sunday)
        {
            keyboard.AddButton($"На завтра ({date:dd.MM - dddd})", CoreKeyboardButtonColor.Primary,
                               "schedule", date.ToString(DefaultDateFormat));
            keyboard.AddLine();
        }

        for(var i = 1; i < 7; i++)
        {
            date = date.AddDays(1);
            if(date.DayOfWeek == DayOfWeek.Sunday)
            {
                continue;
            }

            keyboard.AddButton($"На {date:dd.MM (dddd)}", CoreKeyboardButtonColor.Primary,
                               "schedule", date.ToString(DefaultDateFormat));
            if(i % 2 == 0)
            {
                keyboard.AddLine();
            }
        }

        return keyboard.AddReturnToMenuButton(false);
    }

    public static CoreKeyboard GetDailyWeatherKeyboard()
    {
        var date = DateTime.Now;
        var tomorrow = date.AddDays(1);

        var kb = new CoreKeyboard
        {
            IsInline = true
        };
        kb.AddButton("На сегодня", CoreKeyboardButtonColor.Primary,
                     "weatherDaily", date.ToString(DefaultDateFormat))
          .AddLine()
          .AddButton("На завтра", CoreKeyboardButtonColor.Primary,
                     "weatherDaily", tomorrow.ToString(DefaultDateFormat))
          .AddReturnToMenuButton();

        return kb;
    }
}