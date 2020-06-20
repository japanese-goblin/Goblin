using Goblin.Application.Core.Models;

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
              .AddButton("Погода на текущий момент", CoreKeyboardButtonColor.Primary, "weatherNow", string.Empty)
              .AddButton("Погода на день", CoreKeyboardButtonColor.Primary, "weatherDailyKeyboard", string.Empty)
              .AddLine()
              .AddButton("Рассылка", CoreKeyboardButtonColor.Primary, "mailingKeyboard", string.Empty)
              .AddButton("Напоминания", CoreKeyboardButtonColor.Primary, "reminds", string.Empty)
              .AddButton("Справка", CoreKeyboardButtonColor.Primary, "help", string.Empty);

            return kb;
        }
    }
}