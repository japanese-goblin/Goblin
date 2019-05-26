using System.Collections.Generic;
using System.Threading.Tasks;
using Goblin.Bot.Enums;
using Goblin.Bot.Models;
using Vk.Models.Keyboard;
using Vk.Models.Messages;

namespace Goblin.Bot.Commands
{
    public class Keyboard : ICommand
    {
        public string Name { get; } = "Клавиатура";
        public string Decription { get; } = "Получить клавиатуру";
        public string Usage { get; } = "Клавиатура";
        public string[] Allias { get; } = { "клавиатура", "клава", "кб" };
        public CommandCategory Category { get; } = CommandCategory.Common;
        public bool IsAdmin { get; } = false;

        public Task<CommandResponse> Execute(Message msg)
        {
            var keyboard = new Vk.Models.Keyboard.Keyboard(true);
            string text;
            if(msg.GetParams().ToLower().Contains("убрать"))
            {
                text = "Клавиатура убрана";
                keyboard.Buttons = new List<List<Button>>();
            }
            else
            {
                text = "Вот тебе клавиатура";
                keyboard = GenerateKeyboard();
            }

            return Task.Run(() => new CommandResponse
            {
                Text = text,
                Keyboard = keyboard
            });
        }

        public (bool Success, string Text) CanExecute(Message msg)
        {
            return (true, "");
        }

        private Vk.Models.Keyboard.Keyboard GenerateKeyboard()
        {
            var kb = new Vk.Models.Keyboard.Keyboard(false);

            kb.AddButton("Расписание", ButtonColor.Primary, "cmd", "schedule");
            kb.AddButton("Расписание завтра", ButtonColor.Primary, "cmd", "schedule_tomorrow");
            kb.AddButton("Экзамены", ButtonColor.Primary, "cmd", "exams");
            kb.AddLine();
            kb.AddButton("Погода", ButtonColor.Primary, "cmd", "weather");
            kb.AddButton("Погода завтра", ButtonColor.Primary, "cmd", "weather");
            kb.AddLine();
            kb.AddButton("Напоминания", ButtonColor.Default, "cmd", "reminds");
            kb.AddButton("Команды", ButtonColor.Default, "cmd", "commands");
            return kb;
        }
    }
}