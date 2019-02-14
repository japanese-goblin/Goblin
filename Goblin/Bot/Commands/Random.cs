using System;
using System.Threading.Tasks;
using Goblin.Data.Models;
using Goblin.Models;
using Vk.Models.Messages;

namespace Goblin.Bot.Commands
{
    public class Random : ICommand
    {
        public string Name { get; } = "Рандом *smth*, *smth*, *smth*....";
        public string Decription { get; } = "Выбирает один из нескольких вариантов.";
        public string Usage { get; } = "Рандом 1, 2, 3,4 или 5";
        public string[] Allias { get; } = { "рандом" };
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

            var forRandom = Split(msg.GetParams());
            var index = GetRandom(0, forRandom.Length);
            return new CommandResponse
            {
                Text = $"Я выбираю следующее: {forRandom[index]}"
            };
        }

        public (bool Success, string Text) CanExecute(Message msg)
        {
            var param = msg.GetParams();
            if(string.IsNullOrEmpty(param))
            {
                return (false, $"Ошибка. Пример использования команды: {Usage}");
            }

            var forRandom = Split(param);
            if(forRandom.Length < 2)
            {
                return (false, $"Введи два или более параметра ({Usage})");
            }

            return (true, "");
        }

        private int GetRandom(int start, int end)
        {
            //todo: шо за магическое число
            return new System.Random(DateTime.Now.Millisecond * 3819).Next(start, end);
        }

        private string[] Split(string str)
        {
            return str.Split(new[] { ",", ", ", " или " }, StringSplitOptions.None);
        }
    }
}
