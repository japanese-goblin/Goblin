﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Goblin.Vk.Models;

namespace Goblin.Bot.Commands
{
    public class FlipCommand : ICommand
    {
        public string Name { get; } = "Монета";
        public string Decription { get; } = "Подбрасывает монету и выдаёт орёл/решка";
        public string Usage { get; } = "Монета";
        public List<string> Allias { get; } = new List<string> {"монета"};
        public Category Category { get; } = Category.Common;
        public bool IsAdmin { get; } = false;

        public string Message { get; set; }
        public Keyboard Keyboard { get; set; }

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
            return new Random(DateTime.Now.Millisecond * 3819).Next(start, end);
        }
    }
}