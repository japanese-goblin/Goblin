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
        public string Decription { get; } = "Возвращает расписание на указанную дату. День и месяц обязательно должны содержать 2 цифры.";
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
            Result = GetSchedule(time, group);
        }

        private string GetSchedule(DateTime date, short usergroup)
        {
            var result = $"Расписание на {date:dd.MM}:\n";
            string calen;
            using (var client = new WebClient())
            {
                try
                {
                    client.Encoding = Encoding.UTF8;
                    calen = client.DownloadString(
                        $"http://ruz.narfu.ru/?icalendar&oid={usergroup}&from={DateTime.Now:dd.MM.yyyy}");
                }
                catch (WebException e)
                {
                    return $"Какая-то ошибочка ({e.Message}). Напиши @id***REMOVED*** (сюда) для решения проблемы!!";
                }
            }

            var calendar = Calendar.Load(calen);
            var events = calendar.Events.Where(x => x.Start.Date == date).Distinct().OrderBy(x => x.Start.Value).ToList();
            if (!events.Any()) return $"На {date:dd.MM} расписание отсутствует!";
            foreach (var ev in events)
            {
                var a = ev.Description.Split('\n');
                var time = a[0].Replace('п', ')');
                var group = a[1].Substring(3);
                var temp = a[5].Split('/');
                result += $"{time} - {a[2]} ({a[3]})\nУ группы {group}\n В аудитории {temp[1]} ({temp[0]})\n\n";
            }

            return result;
        }
    }
}