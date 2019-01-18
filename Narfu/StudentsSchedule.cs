using Narfu.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Calendar = Ical.Net.Calendar;

namespace Narfu
{
    public static class StudentsSchedule
    {
        public static Group[] Groups;
        private static readonly string StudentsEndPoint;

        static StudentsSchedule()
        {
            StudentsEndPoint = $"{Utils.EndPoint}/?icalendar";
            Groups = JsonConvert.DeserializeObject<Group[]>(File.ReadAllText("Data/Groups.json"));
        }

        /// <summary>
        /// Получение всего доступного расписания для группы
        /// </summary>
        /// <param name="realGroup">Реальный номер группы</param>
        /// <returns>(произошла ли ошибка, массив с парамаи)</returns>
        public static async Task<(bool IsError, Lesson[] Lessons)> GetSchedule(int realGroup)
        {
            var userGroup = GetGroupByRealId(realGroup).SiteId;
            var response = await Utils.Client.GetAsync($"{StudentsEndPoint}&oid={userGroup}&from={DateTime.Now:dd.MM.yyyy}");
            if (!response.IsSuccessStatusCode)
            {
                return (true, new Lesson[] { });
            }

            Calendar calendar;
            try
            {
                calendar = Calendar.Load(await response.Content.ReadAsStringAsync());
            }
            catch // редирект на главную
            {
                return (true, new Lesson[] { }); //TODO: true -> false?
            }

            var events = calendar.Events.Distinct().OrderBy(x => x.Start.Value).ToList();
            if (!events.Any())
            {
                return (false, new Lesson[] { });
            }

            var lessons = new List<Lesson>();
            foreach (var ev in events)
            {
                var descr = ev.Description.Split('\n');
                var adr = ev.Location.Split('/');
                var les = new Lesson
                {
                    Address = adr[0],
                    Auditory = adr[1],
                    Groups = descr[1].Substring(3),
                    Name = descr[2],
                    Teacher = descr[4],
                    Time = ev.Start.AsSystemLocal,
                    Type = descr[3],
                    StartEndTime = descr[0].Replace(")", "").Replace("(", "").Replace("п", ")"),
                    Number = (byte)descr[0].ElementAt(0)
                };
                lessons.Add(les);
            }

            return (false, lessons.ToArray());
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
                return "Ошибка :с\n" +
                       "Возможно, сайт с расписанием недоступен (либо ошибка на стороне бота)\n" +
                       $"Вы можете проверить расписание здесь: http://ruz.narfu.ru/?timetable&group={group}";
            }

            var lessons = res.Lessons.Where(x => x.Time.DayOfYear == date.DayOfYear).ToList();

            if (lessons.Count == 0)
            {
                return $"На {date:dd.MM (dddd)} расписание отсутствует!";
            }

            var result = $"Расписание на {date:dd.MM (dddd)}:\n";
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
                    x.Type.ToLower().Contains("экзамен") || x.Type.ToLower().Contains("зачет"))
                .OrderBy(x => x.Time)
                .ToList();

            if (lessons.Count == 0)
            {
                return "На данный момент список экзаменов отсутствует";
            }

            var result = "Список экзаменов:\n";
            foreach (var exam in lessons.GroupBy(x => x.Name))
            {
                var f = exam.First();
                var l = exam.Last();
                result += $"{l.Time:dd.MM.yyyy (dddd)} ({f.Time:HH:mm} - {f.StartEndTime.Split("-")[1]})" +
                          $" - {l.Name} [{l.Type}] ({l.Teacher})\n" +
                          $"У группы {l.Groups}\n" +
                          $"В аудитории {l.Auditory}\n\n";
            }

            return result;
        }

        #region utils
        public static int GetWeekNumber(DateTime date)
        {
            var ciCurr = CultureInfo.CurrentCulture;
            var weekNum = ciCurr.Calendar.GetWeekOfYear(date, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
            return weekNum;
        }

        public static bool IsCorrectGroup(int group) => Groups.Any(x => x.RealId == group);

        public static Group GetGroupByRealId(int realId) => Groups.FirstOrDefault(x => x.RealId == realId);

        public static Group GetGroupBySiteId(short siteId) => Groups.FirstOrDefault(x => x.SiteId == siteId);
        #endregion
    }
}