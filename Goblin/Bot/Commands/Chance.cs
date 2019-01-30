using System;
using System.Threading.Tasks;
using Vk.Models.Messages;

namespace Goblin.Bot.Commands
{
    public class Chance : ICommand
    {
        public string Name { get; } = "Вероятность *событие*";
        public string Decription { get; } = "Возвращает случайную вероятность события";
        public string Usage { get; } = "Вероятность сегодня будет дождь";
        public string[] Allias { get; } = { "вероятность" };
        public Category Category { get; } = Category.Common;
        public bool IsAdmin { get; } = false;

        public async Task<CommandResponse> Execute(Message msg)
        {
            var canExecute = CanExecute(msg);
            if(!canExecute.Success)
            {
                return new CommandResponse
                {
                    Text = canExecute.Text
                };
            }

            return new CommandResponse
            {
                Text = $"Вероятность данного события: {GetRandom(0, 100)}%"
            };
        }

        public (bool Success, string Text) CanExecute(Message msg)
        {
            if(string.IsNullOrEmpty(msg.GetParams()))
            {
                return (false, "Ошибка. Не указано событие, вероятность которого необходимо посчитать");
            }

            return (true, "");
        }

        public static int GetRandom(int start, int end)
        {
            return new System.Random(DateTime.Now.Millisecond * 7).Next(start, end);
        }
    }
}
