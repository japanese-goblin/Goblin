using System;
using System.Threading.Tasks;
using Goblin.Helpers;
using Vk.Models.Messages;

namespace Goblin.Bot.Commands
{
    public class Debug : ICommand
    {
        public string Name { get; } = "Debug";
        public string Decription { get; } = "Инфа для дебага";
        public string Usage { get; } = "инфа";
        public string[] Allias { get; } = { "дебаг", "инфа", "debug", "дебуг" };
        public Category Category { get; } = Category.Common;
        public bool IsAdmin { get; } = true;

        public async Task<CommandResponse> Execute(Message msg)
        {
            var bday = new DateTime(2017, 4, 29, 19, 42, 0);
            var dis = DateTime.Now - bday;
            var uptime = DateTime.Now - Program.StartDate;

            var users = DbHelper.GetUsers().Length;
            var scheduleUsers = DbHelper.GetScheduleUsers().Length;
            var weatherUsers = DbHelper.GetWeatherUsers().Length;

            var text =
                $"Время старта: {Program.StartDate:F}\n" +
                $"Гоблин работает уже {uptime.Hours} часов {uptime.Minutes} минут\n" +
                $"Гоблину уже {dis.Days} дней ({bday:D})\n\n" +
                $"Всего пользователей {users}, из которых {scheduleUsers} подписаны на рассылку расписание и {weatherUsers} подписаны на рассылку погоды";

            return new CommandResponse
            {
                Text = text
            };
            //TODO: дополнить чем-нибудь интересным
        }

        public (bool Success, string Text) CanExecute(Message msg)
        {
            return (true, "");
        }

        //private string GetSysUptime()
        //{
        //    var upTime = Environment.TickCount / 1000;
        //    return $"{upTime / 86400} дней {upTime / 3600 % 24} часов {upTime / 60 % 60} минут {upTime % 60} секунд";
        //}
    }
}
