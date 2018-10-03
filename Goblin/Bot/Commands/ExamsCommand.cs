using Goblin.Helpers;
using Goblin.Models.Keyboard;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Goblin.Bot.Commands
{
    public class ExamsCommand : ICommand
    {
        public string Name { get; } = "Экзамены";
        public string Decription { get; } = "Возвращает список экзаменов/зачетов/интернет-экзаменов";
        public string Usage { get; } = "Экзамены";
        public List<string> Allias { get; } = new List<string> { "экзамены" };
        public Category Category { get; } = Category.SAFU;
        public bool IsAdmin { get; } = false;

        public string Message { get; set; }
        public Keyboard Keyboard { get; set; }

        public async Task Execute(string param, int id = 0)
        {
            var user = await DbHelper.Db.Users.FirstOrDefaultAsync(x => x.Vk == id);
            var res = await ScheduleHelper.GetSchedule(user.Group);
            if (res.IsError)
            {
                Message = "Какая-то ошибочка. Возможно, сайт с расписанием недоступен";
                return;
            }

            var lessons = res.Lessons.Distinct().Where(x =>
                              x.Type.Contains("Экзамен") || x.Type.Contains("Зачет") ||
                              x.Type.Contains("Интернет"))
                          .OrderBy(x => x.Time)
                          .ToList();

            if (lessons.Count == 0)
            {
                Message = "На данный момент список экзаменов отсутствует";
                return;
            }

            var result = "Список экзаменов:\n";

            foreach (var l in lessons)
            {
                result += $"{l.Time:dd.MM HH:mm} - {l.Name} ({l.Type})\nУ группы {l.Groups}\n В аудитории {l.Auditory}\n\n";
            }

            Message = result;
        }

        public bool CanExecute(string param, int id = 0)
        {
            var user = DbHelper.Db.Users.First(x => x.Vk == id);
            if (user.Group == 0)
            {
                Message = "Чтобы воспользоваться командой, установи группу командой 'устгр *номер группы*'";
                return false;
            }

            return true;
        }
    }
}