using System.Collections.Generic;

namespace Goblin.Bot.Commands
{
    public class RandomCommand : ICommand
    {
        public string Name { get; } = "Рандом *smth* или *smth*";
        public string Decription { get; } = "Выбирает один из данных вариантов";
        public string Usage { get; } = "Рандом 1 или 2";
        public List<string> Allias { get; } = new List<string>() { "рандом" };
        public Category Category { get; } = Category.Common;
        public bool IsAdmin { get; } = false;

        public string Result { get; set; }

        public void Execute(string param, int id = 0)
        {
            if (string.IsNullOrEmpty(param))
            {
                Result = $"Пример использования команды: {Usage}";
                return;
            }

            var forRandom = param.Split(" или ");
            if (forRandom.Length != 2)
            {
                Result = "Введи два параметра";
                return;
            }

            Result = forRandom[GetRandom(0, 1)];
        }

        public static int GetRandom(int start, int end)
        {
            return new System.Random(System.DateTime.Now.Millisecond * 7).Next(start, end);
        }
    }
}