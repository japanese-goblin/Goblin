using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Narfu.Models;
using Newtonsoft.Json;

namespace Narfu
{
    public static class TeachersSchedule
    {
        public static Teacher[] Teachers;

        static TeachersSchedule()
        {
            var path = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            Teachers = JsonConvert.DeserializeObject<Teacher[]>(File.ReadAllText($"{path}/Data/Teachers.json"));
        }

        public static async Task<(bool IsError, Lesson[] Lessons)> GetScheule(int id)
        {
            HtmlDocument doc;
            var lessons = new List<Lesson>();
            try
            {
                var web = new HtmlWeb
                {
                    UserAgent = Utils.UserAgent
                };
                doc = await web.LoadFromWebAsync($"https://ruz.narfu.ru/?timetable&lecturer={id}");
            }
            catch
            {
                return (true, lessons.ToArray());
            }

            var v = doc.DocumentNode.SelectNodes("//div[contains(@class, 'timetable_sheet')]");
            if(v is null)
            {
                return (true, lessons.ToArray()); //TODO некорректный ид (выкинуло на главную страницу)
            }

            foreach(var les in v)
            {
                var nodes = les.ChildNodes;
                if(nodes.Count <= 3)
                {
                    continue; // пустая ячейка
                }

                var date = les.ParentNode.ChildNodes.FirstOrDefault(x => x.HasClass("dayofweek"))?.InnerText
                              .Trim()
                              .Replace("\n", "").Split(',')[1]
                              .Trim(); // TODO: шо за ужас...

                var adr = nodes.FirstOrDefault(x => x.HasClass("auditorium"))?.InnerText
                               .Trim()
                               .Replace("&nbsp;", " ")
                               .Replace("\n", "").Split(',', 2);

                var time = nodes.FirstOrDefault(x => x.HasClass("time_para"))?.InnerText
                                .Replace("\n", "")
                                .Replace("&ndash;", "-")
                                .Trim();

                lessons.Add(new Lesson
                {
                    Address = adr[1].Trim(),
                    Auditory = adr[0],
                    Groups = nodes.FirstOrDefault(x => x.HasClass("group"))?.InnerText,
                    Name = nodes.FirstOrDefault(x => x.HasClass("discipline"))?.InnerText.Trim(),
                    Number = byte.Parse(nodes.FirstOrDefault(x => x.HasClass("num_para"))?.InnerText),
                    StartEndTime = time,
                    Teacher = "",
                    Time = DateTime.ParseExact(date, "dd.MM.yyyy", null, DateTimeStyles.None),
                    Type = nodes.FirstOrDefault(x => x.HasClass("kindOfWork"))?.InnerText
                });
            }

            return (false, lessons.Distinct().ToArray());
        }

        public static async Task<string> GetScheduleToSend(int id)
        {
            //TODO ?
            var teacher = Teachers.FirstOrDefault(x => x.Id == id);
            if(teacher is null)
            {
                return "Ошибка :с\n" +
                       "Данного преподавателя нет в списке\n" +
                       "Если Вы уверены, что всё правильно, напишите мне для добавления препода в список (https://vk.com/id***REMOVED***)";
            }

            var (error, lessons) = await GetScheule(id);

            if(error)
            {
                return "Ошибка :с\n" +
                       "Возможно, сайт с расписанием недоступен (либо введен неправильный номер преподавателя)\n" +
                       $"Вы можете проверить расписание здесь: https://ruz.narfu.ru/?timetable&lecturer={id}";
            }

            if(lessons.Length == 0)
            {
                return "На данные момент у этого преподавателя нет пар";
            }

            var strBuilder = new StringBuilder();

            strBuilder.AppendFormat("Расписание пар у преподавателя '{0}':", teacher.Name).AppendLine();
            foreach(var group in lessons.Where(x => x.Time.Date >= DateTime.Now.Date)
                                        .GroupBy(x => x.Time.DayOfYear).Take(10))
            {
                strBuilder.AppendFormat("{0:dd.MM (dddd)}", group.First().Time).AppendLine();
                foreach(var lesson in group)
                {
                    strBuilder.AppendFormat("{0} - {1} [{2}]", lesson.StartEndTime, lesson.Name, lesson.Type)
                              .AppendLine();
                    strBuilder.AppendFormat("В {0} ({1})", lesson.Auditory, lesson.Address).AppendLine();
                }

                strBuilder.AppendLine();
            }

            return strBuilder.ToString();
        }

        public static string FindByName(string name)
        {
            name = name.ToLower(); //TODO ?
            var teachers = Teachers.Where(x => x.Name.ToLower().Contains(name));
            return string.Join("\n", teachers
                                    .Select(x => $"{x.Name} ({x.Depart}) - {x.Id}"));
        }

        public static bool FindById(int id)
        {
            return Teachers.Any(x => x.Id == id);
        }
    }
}
