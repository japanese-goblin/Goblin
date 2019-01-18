using System;
using System.Threading.Tasks;
using Vk.Models.Keyboard;
using Vk.Models.Messages;

namespace Goblin.Bot.Commands
{
    public class Flip : ICommand
    {
        public string Name { get; } = "Монета";
        public string Decription { get; } = "Подбрасывает монету и выдаёт орёл/решка";
        public string Usage { get; } = "Монета";
        public string[] Allias { get; } = {"монета"};
        public Category Category { get; } = Category.Common;
        public bool IsAdmin { get; } = false;

        public string Message { get; set; }
        public Keyboard Keyboard { get; set; }

        public async Task Execute(Message msg)
        {
            var forRandom = new[] {"Орёл", "Решка"};

            var a = GetRandom(0, 100);
            Message = forRandom[a % 2 == 0 ? 0 : 1];
        }

        public bool CanExecute(Message msg)
        {
            return true;
        }

        public static int GetRandom(int start, int end)
        {
            return new System.Random(DateTime.Now.Millisecond).Next(start, end);
        }
    }
}