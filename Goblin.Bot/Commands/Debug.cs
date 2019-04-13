using System;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Goblin.Bot.Enums;
using Goblin.Bot.Models;
using Goblin.Persistence;
using Vk.Models.Messages;

namespace Goblin.Bot.Commands
{
    public class Debug : ICommand
    {
        public string Name { get; } = "Debug";
        public string Decription { get; } = "Инфа для дебага";
        public string Usage { get; } = "инфа";
        public string[] Allias { get; } = { "дебаг", "инфа", "debug", "дебуг" };
        public CommandCategory Category { get; } = CommandCategory.Common;
        public bool IsAdmin { get; } = true;

        private readonly MainContext _db;

        public Debug(MainContext db)
        {
            _db = db;
        }

        public Task<CommandResponse> Execute(Message msg)
        {
            var bday = new DateTime(2017, 4, 29, 19, 42, 0);
            var dis = DateTime.Now - bday;

            var startTime = Process.GetCurrentProcess().StartTime;
            var uptime = DateTime.Now - startTime;

            var users = _db.GetUsers().Length;
            var scheduleUsers = _db.GetScheduleUsers().Length;
            var weatherUsers = _db.GetWeatherUsers().Length;

            var strBuilder = new StringBuilder();
            strBuilder.AppendFormat("Время старта: {0:F}", startTime).AppendLine();
            strBuilder.AppendFormat("Гоблин работает уже {0} часов {1} минут", uptime.Hours, uptime.Minutes)
                      .AppendLine();
            strBuilder.AppendFormat("Гоблину уже {0} дней ({1:dd.MM.yyyy})", dis.Days, bday).AppendLine().AppendLine();
            strBuilder.AppendFormat("Всего пользователей {0} ({1} расписание и {2} погода)", users, scheduleUsers,
                                    weatherUsers);

            return Task.Run(() => new CommandResponse
            {
                Text = strBuilder.ToString()
            });

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