using System.Collections.Generic;
using System.Linq;

namespace Goblin.Bot.Commands
{
    public class GetRemindsCommand : ICommand
    {
        public string Name { get; } = "Напоминания";
        public string Decription { get; } = "Возвращает список с созданными напоминаниями";
        public string Usage { get; } = "Напоминания";
        public List<string> Allias { get; } = new List<string> {"напоминания"};
        public Category Category { get; } = Category.Common;
        public bool IsAdmin { get; } = false;
        public string Result { get; set; }

        public void Execute(string param, int id = 0)
        {
            var reminds = "Список напоминаний:\n";
            var ureminds = Utils.DB.Reminds.Where(x => x.VkID == id).OrderBy(x => x.Date).ToList();
            if (!ureminds.Any())
            {
                Result = "Напоминаний нет.";
                return;
            }

            foreach (var rem in ureminds)
            {
                var d = rem.Date.AddHours(3);
                reminds += $"{d:dd.MM.yyyy HH:mm} - {rem.Text}\n";
            }

            Result = reminds;
        }

        public bool CanExecute(string param, int id = 0)
        {
            return true;
        }

        //public string UnixTimeStampToDateTime(double unixTimeStamp)
        //{
        //    var dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        //    dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
        //    dtDateTime = dtDateTime.AddHours(-3);
        //    return $"{dtDateTime:dd.MM.yyyy HH:mm:ss}";
        //}
    }
}