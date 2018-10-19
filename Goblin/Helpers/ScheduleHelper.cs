using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Goblin.Models;
using Newtonsoft.Json;
using Calendar = Ical.Net.Calendar;

namespace Goblin.Helpers
{
    public static class ScheduleHelper
    {
        public static List<Group> Groups;

        static ScheduleHelper()
        {
            Groups = JsonConvert.DeserializeObject<List<Group>>(File.ReadAllText("Groups.json"));
        }

        /// <summary>
        /// Получение всего доступного расписания для группы
        /// </summary>
        /// <param name="realGroup">Реальный номер группы</param>
        /// <returns>(произошла ли ошибка, массив с парамаи)</returns>
        public static async Task<(bool IsError, List<Lesson> Lessons)> GetSchedule(int realGroup)
        {
            var usergroup = GetGroupByRealId(realGroup).SiteId;
            string calen;
            using (var client = new WebClient())
            {
                try
                {
                    client.Encoding = Encoding.UTF8;
                    calen = await client.DownloadStringTaskAsync($"http://ruz.narfu.ru/?icalendar&oid={usergroup}&from={DateTime.Now:dd.MM.yyyy}");
                }
                catch (WebException)
                {
                    return (true, new List<Lesson>());
                }
            }

            Calendar calendar;
            try
            {
                calendar = Calendar.Load(calen);
            }
            catch
            {
                return (true, new List<Lesson>()); //TODO: true -> false?
            }

            var lessons = new List<Lesson>();
            var events = calendar.Events.Distinct().OrderBy(x => x.Start.Value).ToList();
            if (!events.Any())
            {
                return (false, new List<Lesson>());
            }

            foreach (var ev in events)
            {
                var a = ev.Description.Split('\n');
                var adr = ev.Location.Split('/');
                var les = new Lesson
                {
                    Address = adr[0],
                    Auditory = adr[1],
                    Groups = a[1].Substring(3),
                    Name = a[2],
                    Teacher = a[4],
                    Time = ev.Start.AsSystemLocal,
                    Type = a[3],
                    StartEndTime = a[0].Replace(")", "").Replace("(", "").Replace("п", ")"),
                    Number = (byte) a[0].ElementAt(0)
                };
                lessons.Add(les);
            }

            return (false, lessons);
        }

        /// <summary>
        /// Получение расписания в строком виде
        /// </summary>
        /// <param name="date">Дата, на которую нужно расписание</param>
        /// <param name="realGroup">Реальный номер группы сафу</param>
        /// <returns></returns>
        public static async Task<string> GetScheduleAtDate(DateTime date, int realGroup)
        {
            var res = await GetSchedule(realGroup);

            if (res.IsError)
            {
                var group = GetGroupByRealId(realGroup).SiteId;
                return "Какая-то ошибочка :с\n" +
                       "Возможно, сайт с расписанием недоступен (либо ошибка на стороне бота, но это вряд ли)\n" +
                       $"Вы можете проверить расписание здесь: http://ruz.narfu.ru/?timetable&group={group}";
            }

            var lessons = res.Lessons.Where(x => x.Time.DayOfYear == date.DayOfYear).ToList();

            if (lessons.Count == 0) return $"На {date:dd.MM} расписание отсутствует!";

            var result = $"Расписание на {date:dd.MM}:\n";
            foreach (var lesson in lessons.Where(x => x.Time.DayOfYear == date.DayOfYear))
            {
                result += $"{lesson.StartEndTime} - {lesson.Name} [{lesson.Type}] ({lesson.Teacher})\n" +
                          $"У группы {lesson.Groups}\n" +
                          $"В аудитории {lesson.Auditory} ({lesson.Address})\n\n";
            }

            return result;
        }

        /// <summary>
        /// Получение экзаменов в строковом виде
        /// </summary>
        /// <param name="realGroup">Реальный номер группы</param>
        /// <returns>Список экзаменов</returns>
        public static async Task<string> GetExams(int realGroup)
        {
            var res = await GetSchedule(realGroup);

            if (res.IsError)
            {
                var group = GetGroupByRealId(realGroup).SiteId;
                return "Какая-то ошибочка :с\n" +
                       "Возможно, сайт с расписанием недоступен (либо ошибка на стороне бота, но это вряд ли)\n" +
                       $"Вы можете проверить расписание здесь: http://ruz.narfu.ru/?timetable&group={group}";
            }

            var lessons = res.Lessons.Where(x =>
                    x.Type.Contains("Экзамен") || x.Type.Contains("Зачет") ||
                    x.Type.Contains("Интернет"))
                .OrderBy(x => x.Time)
                .ToList();

            if (lessons.Count == 0)
            {
                return "На данный момент список экзаменов отсутствует";
            }

            var result = "Список экзаменов:\n" +
                         string.Join("\n\n",
                             lessons.Select(l => $"{l.Time:dd.MM HH:mm} - {l.Name} ({l.Type})\n" +
                                                 $"У группы {l.Groups}\n" +
                                                 $"В аудитории {l.Auditory}"));

            return result;
        }

        public static int GetWeekNumber(DateTime date)
        {
            var ciCurr = CultureInfo.CurrentCulture;
            var weekNum = ciCurr.Calendar.GetWeekOfYear(date, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
            return weekNum;
        }

        public static bool IsCorrectGroup(int group)
        {
            return Groups.Any(x => x.RealId == group);
        }

        public static Group GetGroupByRealId(int realId)
        {
            return Groups.FirstOrDefault(x => x.RealId == realId);
        }

        public static Group GetGroupBySiteId(short siteId)
        {
            return Groups.FirstOrDefault(x => x.SiteId == siteId);
        }
    }
}