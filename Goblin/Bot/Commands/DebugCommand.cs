using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Goblin.Helpers;
using Vk.Models.Keyboard;

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

        public string Message { get; set; }
        public Keyboard Keyboard { get; set; }

        public async Task Execute(string param, long id = 0)
        {
            var bday = new DateTime(2017, 4, 29, 19, 42, 0);
            var dis = DateTime.Now - bday;
            var uptime = DateTime.Now - Program.StartDate;

            var users = DbHelper.GetUsers().Count;
            var scheduleUsers = DbHelper.GetScheduleUsers().Count;
            var weatherUsers = DbHelper.GetWeatherUsers().Count;

            Message =
                $"Время старта: {Program.StartDate:F}\n" +
                $"Гоблин работает уже {uptime.Hours} часов {uptime.Minutes} минут\n" +
                $"Гоблину уже {dis.Days} дней ({bday:D})\n\n" +
                $"Всего пользователей {users}, из которых {scheduleUsers} подписаны на рассылку расписание и {weatherUsers} подписаны на рассылку погоды";

            //TODO: дополнить чем-нибудь интересным
        }

        public bool CanExecute(string param, long id = 0)
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