using System;
using System.Threading.Tasks;
using Goblin.Data.Models;
using Goblin.Data.Enums;
using Vk.Models.Messages;

namespace Goblin.Bot.Commands
{
    public class Flip : ICommand
    {
        public string Name { get; } = "Монета";
        public string Decription { get; } = "Подбрасывает монету и выдаёт орёл/решка";
        public string Usage { get; } = "Монета";
        public string[] Allias { get; } = { "монета" };
        public Category Category { get; } = Category.Common;
        public bool IsAdmin { get; } = false;

        public async Task<CommandResponse> Execute(Message msg)
        {
            var choices = new[] { "Орёл", "Решка" };

            var a = GetRandom(0, 1);
            return new CommandResponse
            {
                Text = choices[a]
            };
        }

        public (bool Success, string Text) CanExecute(Message msg)
        {
            return (true, "");
        }

        public static int GetRandom(int start, int end)
        {
            return new System.Random(DateTime.Now.Millisecond).Next(start, end);
        }
    }
}