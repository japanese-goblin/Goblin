using System;
using System.Collections.Generic;

namespace Goblin.Bot.Commands
{
    public class RandomCommand : ICommand
    {
        public string Name { get; } = "Рандом *smth* или *smth*";
        public string Decription { get; } = "Выбирает один из данных вариантов";
        public string Usage { get; } = "Рандом 1 или 2";
        public List<string> Allias { get; } = new List<string> {"рандом"};
        public Category Category { get; } = Category.Common;
        public bool IsAdmin { get; } = false;

        public string Result { get; set; }

        public void Execute(string param, int id = 0)
        {
            var forRandom = param.Split(" или ", 2); //TODO: больше вариантов?

            var a = GetRandom(0, 100);
            Result = forRandom[a % 2 == 0 ? 0 : 1];
        }

        public bool CanExecute(string param, int id = 0)
        {
            if (string.IsNullOrEmpty(param))
            {
                Result = $"Пример использования команды: {Usage}";
                return false;
            }

            var forRandom = param.Split(" или ", 2);
            if (forRandom.Length < 2)
            {
                Result = "Введи два параметра";
                return false;
            }
            return true;
        }

        public static int GetRandom(int start, int end)
        {
            return new Random(DateTime.Now.Millisecond * 3819).Next(start, end);
        }
    }
}