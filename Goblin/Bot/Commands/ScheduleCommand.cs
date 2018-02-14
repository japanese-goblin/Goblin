using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using Ical.Net;

namespace Goblin.Bot.Commands
{
    public class ScheduleCommand : ICommand
    {
        public string Name { get; } = "Раписание *день*.*месяц*";
        public string Decription { get; } = "Возвращает расписание на указанную дату";
        public string Usage { get; } = "Расписание 01.02";
        public List<string> Allias { get; } = new List<string>() {"расписание"};
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
            var test = param.Split('.');
            if (!DateTime.TryParse($"{test[1]}.{test[0]}", out var time))
            {
                Result = "Неправильная дата";
                return;
            }
            var group = user.Group;
            Result = GetSchedule(time, group);
        }

        public string GetSchedule(DateTime date, short usergroup)
        {
            var result = $"Расписание на {date:dd.MM}:\n";
            string calen;
            using (var client = new WebClient())
            {
                client.Encoding = Encoding.UTF8;
                calen = client.DownloadString($"http://ruz.narfu.ru/?icalendar&oid={usergroup}&from={DateTime.Now:dd.MM.yyyy}");
            }

            var calendar = Calendar.Load(calen);
            foreach (var ev in calendar.Events.Where(x => x.Start.Date == date))
            {
                /*
                 * ev.Start.Value - время начала пары
                 * ev.Location - 
                 * ev.Description
                 * ev.Summary - название предмета
                 */
                var a = ev.Description.Split('\n');
                /*
                 * 0 - 3п (12:00-13.35)
                 * 1 - Г:(П:) 351617
                 * 2 - название
                 * 3 - тип
                 * 4 - препод
                 * 5 - локация
                 */
                var time = a[0].Replace('п', ')');
                var group = a[1].Substring(3);
                result += $"{time} - {a[2]} ({a[3]})\nУ группы {group}\n в аудитории {a[5]}\n\n";
            }
            return result;
        }
    }
}