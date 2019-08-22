using VkNet.Enums.SafetyEnums;
using VkNet.Model.Keyboard;

namespace Goblin.Application
{
    public static class DefaultKeyboards
    {
        public static MessageKeyboard GetDefaultKeyboard()
        {
            var kb = new KeyboardBuilder();
            kb.AddButton("Расписание", "", KeyboardButtonColor.Primary, "scheduleKeyboard");
            kb.AddButton("Экзамены", "", KeyboardButtonColor.Primary, "exams");
            kb.AddLine();
            kb.AddButton("Погода на текущий момент", "", KeyboardButtonColor.Primary, "weatherNow");
            kb.AddButton("Погода на день", "", KeyboardButtonColor.Primary, "weatherDailyKeyboard");
            kb.AddLine();
            kb.AddButton("Рассылка", "", KeyboardButtonColor.Primary, "mailingKeyboard");
            kb.AddButton("Напоминания", "", KeyboardButtonColor.Default, "reminds");
            kb.AddButton("Справка", "", KeyboardButtonColor.Primary, "help");

            return kb.Build();
        }
    }
}