using VkNet.Enums.SafetyEnums;
using VkNet.Model.Keyboard;

namespace Goblin.Application.Vk
{
    public static class DefaultKeyboards
    {
        public static MessageKeyboard GetDefaultKeyboard()
        {
            var kb = new KeyboardBuilder();
            kb.AddButton("Расписание", "", KeyboardButtonColor.Primary, "scheduleKeyboard")
              .AddButton("Экзамены", "", KeyboardButtonColor.Primary, "exams")
              .AddLine()
              .AddButton("Погода на текущий момент", "", KeyboardButtonColor.Primary, "weatherNow")
              .AddButton("Погода на день", "", KeyboardButtonColor.Primary, "weatherDailyKeyboard")
              .AddLine()
              .AddButton("Рассылка", "", KeyboardButtonColor.Primary, "mailingKeyboard")
              .AddButton("Напоминания", "", KeyboardButtonColor.Default, "reminds")
              .AddButton("Справка", "", KeyboardButtonColor.Primary, "help");
        
            return kb.Build();
        }
    }
}