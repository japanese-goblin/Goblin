using System;
using System.Collections.Generic;

namespace Goblin.Bot.Commands
{
    public class DebugCommand : ICommand
    {
        public string Name { get; } = "Debug";
        public string Decription { get; } = "Инфа для дебага";
        public string Usage { get; } = "инфа";
        public List<string> Allias { get; } = new List<string> {"дебаг", "инфа", "debug", "дебуг"};
        public Category Category { get; } = Category.Common;
        public bool IsAdmin { get; } = true;
        public string Result { get; set; }

        public void Execute(string param, int id = 0)
        {
            var dis = DateTime.Now - new DateTime(2017, 4, 29, 19, 42, 0);
            //TODO: дополнить чем-нибудь интересным
            Result =
                $"Текущее время на сервере: {DateTime.Now:F}\n" +
                $"Аптайм: {GetSysUptime()}\n" +
                $"Гоблину уже {dis.Days} дней {dis.Hours} часов {dis.Minutes} минут!!";
        }

        public bool CanExecute(string param, int id = 0)
        {
            return true;
        }

        private string GetSysUptime()
        {
            var upTime = Environment.TickCount / 1000;
            return $"{upTime / 86400} дней {upTime / 3600 % 24} часов {upTime / 60 % 60} минут {upTime % 60} секунд";
        }
    }
}