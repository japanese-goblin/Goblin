using System.Collections.Generic;

namespace Goblin.Bot.Commands
{
    public class ScheduleCommand : ICommand
    {
        public string Name { get; } = "Раписание";
        public string Decription { get; } = "расписание";
        public string Usage { get; } = "Расписание 01.02";
        public List<string> Allias { get; } = new List<string>() {"расписание"};
        public Category Category { get; } = Category.SAFU;
        public bool IsAdmin { get; } = false;
        public string Result { get; set; }

        public void Execute(string param, int id = 0)
        {
            Result = "Расписание";
        }
    }
}