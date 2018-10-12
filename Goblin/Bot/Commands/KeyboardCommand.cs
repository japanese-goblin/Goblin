using System.Collections.Generic;
using System.Threading.Tasks;
using Goblin.Models.Keyboard;

namespace Goblin.Bot.Commands
{
    public class KeyboardCommand : ICommand

    {
        public string Name { get; } = "клавиатура";
        public string Decription { get; } = "показывает клавиатуру";
        public string Usage { get; } = "клавиатура";
        public List<string> Allias { get; } = new List<string>() {"клавиатура", "клава", "кб"};
        public Category Category { get; } = Category.Common;
        public bool IsAdmin { get; } = false;

        public string Message { get; set; }
        public Keyboard Keyboard { get; set; }

        public async Task Execute(string param, int id = 0)
        {
            Message = "Вот тебе клавиатура";
            Keyboard = GenerateKeyboard();
        }

        public bool CanExecute(string param, int id = 0)
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