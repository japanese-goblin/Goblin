using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Goblin.Helpers;
using Goblin.Models;
using Microsoft.EntityFrameworkCore;

namespace Goblin.Bot.Commands
{
    public class ExamsCommand : ICommand
    {
        public string Name { get; } = "Экзамены";
        public string Decription { get; } = "Возвращает список экзаменов/зачетов/интернет-экзаменов";
        public string Usage { get; } = "Экзамены";
        public List<string> Allias { get; } = new List<string> {"экзамены"};
        public Category Category { get; } = Category.SAFU;
        public bool IsAdmin { get; } = false;

        public string Result { get; set; }

        private MainContext db = new MainContext();

        public async Task Execute(string param, int id = 0)
        {
            var user = await db.Users.FirstOrDefaultAsync(x => x.Vk == id);
            var res = await ScheduleHelper.GetSchedule(user.Group);
            if (res.IsError)
            {
                Result = "Какая-то ошибочка. Возможно сменилась группа на сайте или сайт с расписанием недоступен";
                return;
            }

            var lessons = res.Lessons.Where(x =>
                              x.Type.Contains("Экзамен") || x.Type.Contains("Зачет") ||
                              x.Type.Contains("Интернет"))
                          .Distinct()
                          .OrderBy(x => x.Time)
                          .ToList();

            if (lessons.Count == 0)
            {
                Result = "На данный момент список экзаменов отсутствует";
                return;
            }

            var result = "Список экзаменов:\n";

            foreach (var l in lessons)
            {
                result += $"{l.Time:dd.MM HH:mm} - {l.Name} ({l.Type})\nУ группы {l.Groups}\n В аудитории {l.Address}\n\n";
            }

            Result = result;
        }

        public bool CanExecute(string param, int id = 0)
        {
            var user = db.Users.First(x => x.Vk == id);
            if (user.Group == 0)
            {
                Result = "Установи группу командой 'устгр'";
                return false;
            }

            return true;
        }
    }
}