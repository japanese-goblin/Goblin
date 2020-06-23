using System;
using Goblin.Application.Core.Models;
using Goblin.Domain.Abstractions;

namespace Goblin.Application.Core
{
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
            var isSchedule = user.HasScheduleSubscription;
            var isWeather = user.HasWeatherSubscription;

            var scheduleColor = isSchedule
                                        ? CoreKeyboardButtonColor.Negative
                                        : CoreKeyboardButtonColor.Positive;
            var weatherColor = isWeather
                                       ? CoreKeyboardButtonColor.Negative
                                       : CoreKeyboardButtonColor.Positive;

            var scheduleText = isSchedule
                                       ? "Отписаться от рассылки расписания"
                                       : "Подписаться на рассылку расписания";
            var weatherText = isWeather
                                      ? "Отписаться от рассылки погоды"
                                      : "Подписаться на рассылку погоды";

            var kb = new CoreKeyboard
            {
                IsInline = true
            };
            kb.AddButton(scheduleText, scheduleColor, "mailing", "schedule")
              .AddLine()
              .AddButton(weatherText, weatherColor, "mailing", "weather")
              .AddReturnToMenuButton();

            return kb;
        }

        public static CoreKeyboard GetScheduleKeyboard()
        {
            const string defaultFormat = "dd.MM.yyyy";
            var startDate = DateTime.Now;

            var keyboard = new CoreKeyboard
            {
                IsInline = true
            };
            keyboard.AddButton($"На сегодня ({startDate:dd.MM - dddd})", CoreKeyboardButtonColor.Primary,
                               "schedule", startDate.ToString(defaultFormat));
            keyboard.AddLine();

            startDate = startDate.AddDays(1);
            keyboard.AddButton($"На завтра ({startDate:dd.MM - dddd})", CoreKeyboardButtonColor.Primary,
                               "schedule", startDate.ToString(defaultFormat));
            keyboard.AddLine();

            for(var i = 1; i < 7; i++)
            {
                var date = startDate.AddDays(i);
                keyboard.AddButton($"На {date:dd.MM (dddd)}", CoreKeyboardButtonColor.Primary,
                                   "schedule", date.ToString(defaultFormat));
                if(i % 2 == 0)
                {
                    keyboard.AddLine();
                }
            }

            keyboard.AddReturnToMenuButton(false);

            return keyboard;
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
}