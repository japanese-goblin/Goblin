using System;
using System.Threading.Tasks;
using Vk.Models.Keyboard;

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
        public Vk.Models.Keyboard.Keyboard Keyboard { get; set; }

        public async Task Execute(string param, long id = 0)
        {
            var forRandom = new[] {"Орёл", "Решка"};

            var a = GetRandom(0, 100);
            Message = forRandom[a % 2 == 0 ? 0 : 1];
        }

        public bool CanExecute(string param, long id = 0)
        {
            return true;
        }

        public static int GetRandom(int start, int end)
        {
            return new System.Random(DateTime.Now.Millisecond * 3819).Next(start, end);
        }
    }
}