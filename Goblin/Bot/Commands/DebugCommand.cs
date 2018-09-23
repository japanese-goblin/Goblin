using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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

        public async Task Execute(string param, int id = 0)
        {
            var dis = DateTime.Now - new DateTime(2017, 4, 29, 19, 42, 0);
            var uptime = DateTime.Now - Program.StartDate;
            Result =
                $"Время старта: {Program.StartDate:F}\n" +
                $"Гоблин работает уже {uptime.Hours} часов {uptime.Minutes} минут\n" +
                $"Гоблину уже {dis.Days} дней {dis.Hours} часов {dis.Minutes} минут!!";

            //TODO: дополнить чем-нибудь интересным
        }

        public bool CanExecute(string param, int id = 0)
        {
            return true;
        }

        //private string GetSysUptime()
        //{
        //    var upTime = Environment.TickCount / 1000;
        //    return $"{upTime / 86400} дней {upTime / 3600 % 24} часов {upTime / 60 % 60} минут {upTime % 60} секунд";
        //}
    }
}