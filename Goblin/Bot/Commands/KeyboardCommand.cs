using System.Collections.Generic;
using System.Threading.Tasks;
using VkNet.Enums.SafetyEnums;
using VkNet.Model.Keyboard;

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
        public MessageKeyboard Keyboard { get; set; }

        public async Task Execute(string param, int id = 0)
        {
            Message = "Вот тебе клавиатура";
            Keyboard = GenerateKeyboard();
        }

        public bool CanExecute(string param, int id = 0)
        {
            return true;
        }

        private MessageKeyboard GenerateKeyboard()
        {
            var kb = new KeyboardBuilder(false);

            kb.AddButton("Расписание", "", KeyboardButtonColor.Primary);
            kb.AddButton("Экзамены", "", KeyboardButtonColor.Primary);
            kb.AddLine();
            kb.AddButton("Погода", "", KeyboardButtonColor.Primary);
            kb.AddButton("Монета", "", KeyboardButtonColor.Primary);
            kb.AddLine();
            kb.AddButton("Напоминания", "", KeyboardButtonColor.Primary);
            kb.AddButton("Команды", "", KeyboardButtonColor.Primary);
            return kb.Build();
        }
    }
}