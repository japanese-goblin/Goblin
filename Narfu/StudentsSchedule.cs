using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Narfu.Models;
using Newtonsoft.Json;
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
            var path = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            Groups = JsonConvert.DeserializeObject<Group[]>(File.ReadAllText($"{path}/Data/Groups.json"));
        }

        /// <summary>
        /// Получение всего доступного расписания для группы
        /// </summary>
        /// <param name="realGroup">Реальный номер группы</param>
        /// <returns>(произошла ли ошибка, массив с парамаи)</returns>
        public static async Task<(bool IsError, Lesson[] Lessons)> GetSchedule(int realGroup)
        {
            var userGroup = GetGroupByRealId(realGroup).SiteId;
            var url = $"{StudentsEndPoint}&oid={userGroup}&cod={realGroup}&from={DateTime.Now:dd.MM.yyyy}";
            var response = await Utils.Client.GetAsync(url);
            if(!response.IsSuccessStatusCode)
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
            if(!events.Any())
            {
                return (false, new Lesson[] { });
            }

            var lessons = events.Select(ev =>
            {
                var descr = ev.Description.Split('\n');
                var adr = ev.Location.Split('/');
                return new Lesson
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
            }).ToArray();

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

            if(res.IsError)
            {
                var group = GetGroupByRealId(realGroup).SiteId;
                return "Ошибка :с\n" + "Возможно, сайт с расписанием недоступен (либо ошибка на стороне бота)\n" +
                       $"Вы можете проверить расписание здесь: https://ruz.narfu.ru/?timetable&group={group}";
            }

            var lessons = res.Lessons.Where(x => x.Time.DayOfYear == date.DayOfYear).ToList();

            if(lessons.Count == 0)
            {
                return $"На {date:dd.MM (dddd)} расписание отсутствует!";
            }

            var strBuilder = new StringBuilder();

            strBuilder.AppendFormat("Расписание на {0:dd.MM (dddd)}", date).AppendLine();
            foreach(var lesson in lessons.Where(x => x.Time.DayOfYear == date.DayOfYear))
            {
                strBuilder.AppendFormat("{0} - {1} [{2}] ({3})", lesson.StartEndTime, lesson.Name, lesson.Type, lesson.Teacher)
                          .AppendLine();
                strBuilder.AppendFormat("У группы {0}", lesson.Groups).AppendLine();
                strBuilder.AppendFormat("В аудитории {0} ({1})", lesson.Auditory, lesson.Address).AppendLine();
                strBuilder.AppendLine().AppendLine();
            }

            return strBuilder.ToString();
        }

        /// <summary>
        /// Получение экзаменов в строковом виде
        /// </summary>
        /// <param name="realGroup">Реальный номер группы</param>
        /// <returns>Список экзаменов</returns>
        public static async Task<string> GetExams(int realGroup)
        {
            var res = await GetSchedule(realGroup);

            if(res.IsError)
            {
                var group = GetGroupByRealId(realGroup).SiteId;
                return "Какая-то ошибочка :с\n" +
                       "Возможно, сайт с расписанием недоступен (либо ошибка на стороне бота, но это вряд ли)\n" +
                       $"Вы можете проверить расписание здесь: https://ruz.narfu.ru/?timetable&group={group}";
            }

            var lessons = res.Lessons.Where(x => x.Type.ToLower().Contains("экзамен") || x.Type.ToLower().Contains("зачет"))
                             .OrderBy(x => x.Time).ToArray();

            if(lessons.Length == 0)
            {
                return "На данный момент список экзаменов отсутствует";
            }

            var strBuilder = new StringBuilder();

            strBuilder.AppendLine("Список экзаменов:");
            foreach(var exam in lessons.GroupBy(x => x.Name))
            {
                var f = exam.First();
                var l = exam.Last();
                var endTime = l.StartEndTime.Split("-")[1];
                strBuilder.AppendFormat("{0:dd.MM.yyyy (dddd)} ({1:HH:mm} - {2}) - {3} [{4}] ({5})",
                                        l.Time, f.Time, endTime, l.Name, l.Type, l.Teacher).AppendLine();
                strBuilder.AppendFormat("У группы {0}", l.Groups).AppendLine();
                strBuilder.AppendFormat("В аудитории {0}", l.Auditory).AppendLine();
                strBuilder.AppendLine();
            }

            return strBuilder.ToString();
        }

        #region utils
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
        #endregion
    }
}
