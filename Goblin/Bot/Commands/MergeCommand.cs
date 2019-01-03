using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Goblin.Vk.Models;

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

        public string Message { get; set; }
        public Keyboard Keyboard { get; set; }

        public async Task Execute(string param, long id = 0)
        {
            var str = param.Split(' ');
            var get = (Convert.ToInt32(str[0].Length / 2), Convert.ToInt32(str[1].Length / 2));
            var first = str[0].Substring(0, get.Item1);
            var second = str[1].Substring(get.Item2);
            Message = $"{first}{second}";
        }

        public bool CanExecute(string param, long id = 0)
        {
            if (string.IsNullOrEmpty(param))
            {
                Message = "Введи два слова через пробел";
                return false;
            }

            if (param.Split(' ').Length != 2)
            {
                Message = "можно только 2 слова!!";
                return false;
            }

            return true;
        }
    }
}