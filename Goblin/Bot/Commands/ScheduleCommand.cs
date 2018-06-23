using System;
using System.Collections.Generic;
using System.Linq;

namespace Goblin.Bot.Commands
{
    public class ScheduleCommand : ICommand
    {
        public string Name { get; } = "Раписание *день*.*месяц*";
        public string Decription { get; } = "Возвращает расписание на указанную дату. День и месяц обязательно должны содержать 2 цифры.";
        public string Usage { get; } = "Расписание 01.02";
        public List<string> Allias { get; } = new List<string> {"расписание"};
        public Category Category { get; } = Category.SAFU;
        public bool IsAdmin { get; } = false;
        public string Result { get; set; }

        public void Execute(string param, int id = 0)
        {
            var user = Utils.DB.Users.First(x => x.Vk == id);
            if (user.Group == 0)
            {
                Result = "Для начала установи группу командой 'устгр'";
                return;
            }

            var test = param.Split('.').Select(int.Parse).ToList();
            DateTime time;
            try
            {
                time = new DateTime(2018, test[1], test[0]);
            }
            catch
            {
                Result = "Неправильная дата";
                return;
            }

            var group = user.Group;
            Result = Utils.GetSchedule(time, group);
        }
    }
}