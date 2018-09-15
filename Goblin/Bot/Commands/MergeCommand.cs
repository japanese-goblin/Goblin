using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Goblin.Bot.Commands
{
    public class MergeCommand : ICommand
    {
        public string Name { get; } = "Соедини *слово1* *слово2*";
        public string Decription { get; } = "Соединяет два слова (зачем???)";
        public string Usage { get; } = "Соедини сафу лучший";
        public List<string> Allias { get; } = new List<string> {"соедини"};
        public Category Category { get; } = Category.Common;
        public bool IsAdmin { get; } = false;

        public string Result { get; set; }

        public async Task Execute(string param, int id = 0)
        {
            var str = param.Split(' ');
            var get = (Convert.ToInt32(str[0].Length / 2), Convert.ToInt32(str[1].Length / 2));
            var first = str[0].Substring(0, get.Item1);
            var second = str[1].Substring(get.Item2);
            Result = $"{first}{second}";
        }

        public bool CanExecute(string param, int id = 0)
        {
            if (string.IsNullOrEmpty(param))
            {
                Result = "почому тут пусто??? Введи два параметра";
                return false;
            }

            if (param.Split(' ').Length != 2)
            {
                Result = "можно только 2 параметра!!";
                return false;
            }

            return true;
        }
    }
}