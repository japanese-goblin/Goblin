using System.Threading.Tasks;
using Vk.Models.Keyboard;
using Vk.Models.Messages;

namespace Goblin.Bot.Commands
{
    public class KeyboardCommand : ICommand

    {
        public string Name { get; } = "Клавиатура";
        public string Decription { get; } = "показывает клавиатуру";
        public string Usage { get; } = "Клавиатура";
        public string[] Allias { get; } = {"клавиатура", "клава", "кб"};
        public Category Category { get; } = Category.Common;
        public bool IsAdmin { get; } = false;

        public string Message { get; set; }
        public Keyboard Keyboard { get; set; }

        public async Task Execute(Message msg)
        {
            Message = "Вот тебе клавиатура";
            Keyboard = GenerateKeyboard();
        }

        public bool CanExecute(Message msg)
        {
            return true;
        }

        private Keyboard GenerateKeyboard()
        {
            var kb = new Keyboard(false);

            kb.AddButton("Расписание", ButtonColor.Primary, "cmd", "schedule");
            kb.AddButton("Расписание завтра", ButtonColor.Primary, "cmd", "schedule_tomorrow");
            kb.AddButton("Экзамены", ButtonColor.Negative, "cmd", "exams");
            kb.AddLine();
            kb.AddButton("Погода", ButtonColor.Primary, "cmd", "weather");
            kb.AddButton("Напоминания", ButtonColor.Primary, "cmd", "reminds");
            kb.AddButton("Команды", ButtonColor.Primary, "cmd", "commands");
            return kb;
        }
    }
}