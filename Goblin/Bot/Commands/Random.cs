using System;
using System.Threading.Tasks;
using Vk.Models.Keyboard;
using Vk.Models.Messages;

namespace Goblin.Bot.Commands
{
    public class Random : ICommand
    {
        public string Name { get; } = "Рандом *smth*, *smth*, *smth*....";
        public string Decription { get; } = "Выбирает один из нескольких вариантов.";
        public string Usage { get; } = "Рандом 1, 2, 3,4 или 5";
        public string[] Allias { get; } = {"рандом"};
        public Category Category { get; } = Category.Common;
        public bool IsAdmin { get; } = false;

        public string Message { get; set; }
        public Keyboard Keyboard { get; set; }

        public async Task Execute(Message msg)
        {
            var forRandom = Split(msg.GetParams());

            var index = GetRandom(0, forRandom.Length);
            Message = $"Я выбираю следующее: {forRandom[index]}";
        }

        public bool CanExecute(Message msg)
        {
            var param = msg.GetParams();
            if (string.IsNullOrEmpty(param))
            {
                Message = $"Пример использования команды: {Usage}";
                return false;
            }

            var forRandom = Split(param);
            if (forRandom.Length < 2)
            {
                Message = "Введи два или более параметроы";
                return false;
            }

            return true;
        }

        private int GetRandom(int start, int end)
        {
            //todo: шо за магическое число
            return new System.Random(DateTime.Now.Millisecond * 3819).Next(start, end);
        }

        private string[] Split(string str)
        {
            return str.Split(new[] {",", ", ", " или "}, StringSplitOptions.None);
        }
    }
}