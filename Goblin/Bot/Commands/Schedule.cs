using Goblin.Helpers;
using Microsoft.EntityFrameworkCore;
using Narfu;
using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Vk.Models.Keyboard;
using Vk.Models.Messages;

namespace Goblin.Bot.Commands
{
    public class Schedule : ICommand
    {
        public string Name { get; } = "Раписание *день*.*месяц*";

        public string Decription { get; } =
            "Возвращает расписание на указанную дату. Если дата не указана, расписание берется на текущую дату";

        public string Usage { get; } = "Расписание 21.12";
        public string[] Allias { get; } = { "расписание" };
        public Category Category { get; } = Category.SAFU;
        public bool IsAdmin { get; } = false;

        public string Message { get; set; }
        public Keyboard Keyboard { get; set; }

        public async Task Execute(Message msg)
        {
            var param = msg.GetParams();
            var user = await DbHelper.Db.Users.FirstAsync(x => x.Vk == msg.FromId);
            DateTime time;
            if (param == "")
            {
                time = DateTime.Now;
            }
            else if (param.Trim().ToLower() == "завтра")
            {
                time = DateTime.Now.AddDays(1);
            }
            else
            {
                var dayAndMonth = param.Split('.').Select(int.Parse).ToList(); // [Day, Month]
                time = new DateTime(DateTime.Now.Year, dayAndMonth[1], dayAndMonth[0]);
            }

            Message = await StudentsSchedule.GetScheduleAtDate(time, user.Group);
        }

        public bool CanExecute(Message msg)
        {
            var param = msg.GetParams();
            var user = DbHelper.Db.Users.First(x => x.Vk == msg.FromId);
            if (user.Group == 0)
            {
                Message =
                    "Чтобы воспользоваться расписание установи группу командой 'устгр *номер группы*' (без кавычек и звездочек)";
                return false;
            }

            if (param == "" || param.ToLower() == "завтра")
            {
                return true;
            }

            var date = param.Split('.');
            if (date.Length != 2)
            {
                Message = $"Ошибочка. Пример использования команды: {Usage}";
                return false;
            }

            var isGoodDate = DateTime.TryParseExact($"{date[0]}.{date[1]}",
                new[] { "d.M", "d.MM", "dd.M", "dd.MM" },
                null, DateTimeStyles.None, out _);

            if (!isGoodDate)
            {
                Message = $"Ошибочка. Пример использования команды: {Usage}";
                return false;
            }

            return true;
        }
    }
}