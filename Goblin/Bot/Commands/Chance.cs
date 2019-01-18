using System;
using System.Threading.Tasks;
using Vk.Models.Keyboard;
using Vk.Models.Messages;

namespace Goblin.Bot.Commands
{
    public class Chance : ICommand
    {
        public string Name { get; } = "Вероятность *событие*";
        public string Decription { get; } = "Возвращает случайную вероятность события";
        public string Usage { get; } = "Вероятность сегодня будет дождь";
        public string[] Allias { get; } = {"вероятность"};
        public Category Category { get; } = Category.Common;
        public bool IsAdmin { get; } = false;

        public string Message { get; set; }
        public Keyboard Keyboard { get; set; }

        public async Task Execute(Message msg)
        {
            Message = $"Вероятность данного события: {GetRandom(0, 100)}%";
        }

        public bool CanExecute(Message msg)
        {
            if (string.IsNullOrEmpty(msg.GetParams()))
            {
                Message = "А где вопрос?";
                return false;
            }

            return true;
        }

        public static int GetRandom(int start, int end)
        {
            return new System.Random(DateTime.Now.Millisecond * 7).Next(start, end);
        }
    }
}