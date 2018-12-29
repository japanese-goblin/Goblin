using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Goblin.Models;
using HtmlAgilityPack;
using Newtonsoft.Json;

namespace Goblin.Helpers
{
    public static class TeacherScheduleHelper
    {
        public static Teacher[] Teachers;

        static TeacherScheduleHelper()
        {
            Teachers = JsonConvert.DeserializeObject<Teacher[]>(File.ReadAllText("Data/Teachers.json"));
        }

        public static async Task<(bool IsError, List<Lesson> Lessons)> GetScheule(int id)
        {
            HtmlDocument doc;
            var lessons = new List<Lesson>();
            try
            {
                var web = new HtmlWeb
                {
                    UserAgent =
                        "Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/71.0.3578.98 Safari/537.36"
                };
                doc = await web.LoadFromWebAsync($"https://ruz.narfu.ru/?timetable&lecturer={id}");
            }
            catch
            {
                return (true, lessons);
            }

            var v = doc.DocumentNode.SelectNodes("//div[contains(@class, 'timetable_sheet')]");
            if (v is null) return (true, lessons); //TODO некорректный ид

            foreach (var les in v)
            {
                var nodes = les.ChildNodes;
                if (nodes.Count <= 3) continue; // пустая ячейка

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

                lessons.Add(new Lesson()
                {
                    Address = adr[1].Trim(),
                    Auditory = adr[0],
                    Groups = nodes.FirstOrDefault(x => x.HasClass("group"))?.InnerText,
                    Name = nodes.FirstOrDefault(x => x.HasClass("discipline"))?.InnerText.Trim(),
                    Number = byte.Parse(nodes.FirstOrDefault(x => x.HasClass("num_para"))?.InnerText),
                    StartEndTime = time,
                    Teacher = "", //TODO ?
                    Time = DateTime.ParseExact(date, "dd.MM.yyyy", null, DateTimeStyles.None), //TODO ?
                    Type = nodes.FirstOrDefault(x => x.HasClass("kindOfWork"))?.InnerText
                });
            }

            return (false, lessons.Distinct().ToList());
        }

        public static async Task<string> GetScheduleToSend(int id)
        {
            //TODO ?
            var teacher = Teachers.FirstOrDefault(x => x.Id == id);
            if (teacher is null)
            {
                return "Ошибка!\n" +
                       "Данного преподавателя нет в списке\n" +
                       "Если Вы уверены, что всё правильно, напишите мне для добавления препода в список (https://vk.com/id***REMOVED***)";
            }

            var (error, lessons) = await GetScheule(id);

            if (error)
            {
                return "Какая-то ошибочка :с\n" +
                       "Возможно, сайт с расписанием недоступен (либо введен неправильный номер преподавателя)\n" +
                       $"Вы можете проверить расписание здесь: https://ruz.narfu.ru/?timetable&lecturer={id}";
            }

            if (lessons.Count == 0)
            {
                return "На данные момент у этого преподавателя нет пар";
            }

            var result = $"Расписание пар у препода '{teacher.Name}':\n";
            foreach (var group in lessons.Where(x => x.Time.Date >= DateTime.Now.Date)
                                  .GroupBy(x => x.Time.DayOfYear).Take(10))
            {
                result += $"{group.FirstOrDefault().Time:dd.MM.yyyy}:\n";
                foreach (var lesson in group)
                {
                    result += $"{lesson.StartEndTime} - {lesson.Name} [{lesson.Type}]\n" +
                              // $"{lesson.Groups}\n" + // TODO
                              $"В {lesson.Auditory} ({lesson.Address})\n\n";
                }

                result += "\n\n";
            }

            return result;
        }

        public static string FindByName(string name)
        {
            return string.Join("\n",
                Teachers.Where(x => x.Name.ToLower().Contains(name)).Select(x => $"{x.Name} ({x.Depart}) - {x.Id}"));
        }

        public static bool FindById(int id) => Teachers.Any(x => x.Id == id);
    }
}