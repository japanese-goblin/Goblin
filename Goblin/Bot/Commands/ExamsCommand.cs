using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using Ical.Net;

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

        public void Execute(string param, int id = 0)
        {
            var result = "Список экзаменов:\n";
            var user = Utils.DB.Users.First(x => x.Vk == id);
            if (user.Group == 0)
            {
                Result = "Установи группу командой 'устгр'";
                return;
            }

            string calen;
            using (var client = new WebClient())
            {
                try
                {
                    client.Encoding = Encoding.UTF8;
                    calen = client.DownloadString(
                        $"http://ruz.narfu.ru/?icalendar&oid={user.Group}&from={DateTime.Now:dd.MM.yyyy}");
                }
                catch (WebException e)
                {
                    Result = $"Какая-то ошибочка ({e.Message}). Напиши @id***REMOVED*** (сюда) для решения проблемы!!";
                    return;
                }
            }

            var calendar = Calendar.Load(calen);
            var events = calendar.Events.Where(x =>
                x.Description.Contains("Экзамен") || x.Description.Contains("Зачет") ||
                x.Description.Contains("Интернет")).Distinct().OrderBy(x => x.Start.Value);
            foreach (var ev in events)
            {
                var a = ev.Description.Split('\n');
                var time = a[0].Replace('п', ')');
                var group = a[1].Substring(3);
                result += $"{ev.DtStart.Value.AddHours(3):dd.MM HH:mm} - {a[2]} ({a[3]})\nУ группы {group}\n В аудитории {a[5]}\n\n";
            }

            Result = result;
        }
    }
}