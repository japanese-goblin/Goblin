using Goblin.Application.Core.Models;
using Goblin.Domain.Entities;

namespace Goblin.Application.Core;

public static class DefaultKeyboards
{
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
        const string defaultFormat = "dd.MM.yyyy";
        var today = DateTime.Now;

        var keyboard = new CoreKeyboard
        {
            IsInline = true
        };
        keyboard.AddButton($"На сегодня ({today:dd.MM - dddd})", CoreKeyboardButtonColor.Primary,
                           "schedule", today.ToString(defaultFormat));
        keyboard.AddLine();

        var tomorrow = today.AddDays(1);
        if(tomorrow.DayOfWeek != DayOfWeek.Sunday)
        {
            keyboard.AddButton($"На завтра ({tomorrow:dd.MM - dddd})", CoreKeyboardButtonColor.Primary,
                               "schedule", tomorrow.ToString(defaultFormat));
            keyboard.AddLine();
        }

        for(var i = 1; i < 7; i++)
        {
            var date = tomorrow.AddDays(i);
            if(date.DayOfWeek == DayOfWeek.Sunday)
            {
                continue;
            }

            keyboard.AddButton($"На {date:dd.MM (dddd)}", CoreKeyboardButtonColor.Primary,
                               "schedule", date.ToString(defaultFormat));
            if(i % 2 == 0)
            {
                keyboard.AddLine();
            }
        }

        return keyboard.AddReturnToMenuButton(false);
    }

    public static CoreKeyboard GetDailyWeatherKeyboard()
    {
        const string defaultFormat = "dd.MM.yyyy";
        var today = DateTime.Now;
        var tomorrow = today.AddDays(1);

        var kb = new CoreKeyboard
        {
            IsInline = true
        };
        kb.AddButton("На сегодня", CoreKeyboardButtonColor.Primary,
                     "weatherDaily", today.ToString(defaultFormat))
          .AddLine()
          .AddButton("На завтра", CoreKeyboardButtonColor.Primary,
                     "weatherDaily", tomorrow.ToString(defaultFormat))
          .AddReturnToMenuButton();

        return kb;
    }
}