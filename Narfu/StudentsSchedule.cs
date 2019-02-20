using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
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
        /// Получение всего доступного расписания группы на указанную дату
        /// </summary>
        /// <param name="groupId">
        /// ID группы на сайте
        /// </param>
        /// <returns>
        /// Возвращает кортеж:
        /// Error - произошла ли ошибка при выполнении запроса
        /// Code - код ответа
        /// Lessons - массив с парами
        /// </returns>
        public static async Task<(bool Error, HttpStatusCode Code, Lesson[] Lessons)> GetSchedule(int groupId)
        {
            var siteGroup = GetGroupByRealId(groupId).SiteId;
            var requestUrl = $"{StudentsEndPoint}&oid={siteGroup}&cod={groupId}&from={DateTime.Now:dd.MM.yyyy}";
            var response = await Utils.Client.GetAsync(requestUrl);
            if(!response.IsSuccessStatusCode || response.Content is null)
            {
                return (true, response.StatusCode, null);
            }

            var calendar = Calendar.Load(await response.Content.ReadAsStringAsync());
            var events = calendar.Events
                                 .Distinct()
                                 .OrderBy(x => x.DtStart.Value)
                                 .ToArray();
            if(!events.Any())
            {
                return (false, response.StatusCode, new Lesson[] { });
            }

            var lessons = events.Select(ev =>
            {
                var description = ev.Description.Split('\n');
                var address = ev.Location.Split('/');
                return new Lesson
                {
                    Address = address[0],
                    Auditory = address[1],
                    Number = (byte)description[0].ElementAt(0),
                    Groups = description[1].Substring(3),
                    Name = description[2],
                    Type = description[3],
                    Teacher = description[4],
                    StartTime = ev.DtStart.AsSystemLocal,
                    EndTime = ev.DtEnd.AsSystemLocal,
                    StartEndTime = description[0].Replace(")", "").Replace("(", "").Replace("п", ")"),
                };
            }).ToArray();

            return (false, response.StatusCode, lessons);
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

            if(res.Error)
            {
                var group = GetGroupByRealId(realGroup).SiteId;
                return GenerateErrorMessage(res.Code, group);
            }

            var lessons = res.Lessons.Where(x => x.StartTime.DayOfYear == date.DayOfYear).ToArray();

            if(lessons.Length == 0)
            {
                return $"На {date:dd.MM (dddd)} расписание отсутствует!";
            }

            var strBuilder = new StringBuilder();

            strBuilder.AppendFormat("Расписание на {0:dd.MM (dddd)}", date).AppendLine();
            foreach(var lesson in lessons.Where(x => x.StartTime.DayOfYear == date.DayOfYear))
            {
                strBuilder.AppendFormat("{0} - {1} [{2}] ({3})",
                                        lesson.StartEndTime, lesson.Name, lesson.Type, lesson.Teacher)
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

            if(res.Error)
            {
                var group = GetGroupByRealId(realGroup).SiteId;
                return GenerateErrorMessage(res.Code, group);
            }

            var lessons = res.Lessons.Where(x => x.Type.ToLower().Contains("экзамен") || x.Type.ToLower().Contains("зачет"))
                             .OrderBy(x => x.StartTime).ToArray();

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
                                        l.StartTime, f.StartTime, endTime, l.Name, l.Type, l.Teacher).AppendLine();
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

        private static string GenerateErrorMessage(HttpStatusCode code, int group)
        {
            return $"Ошибка (код ошибки - {code}).\n" +
                   "Сайт с расписанием недоступен (либо сломалось получение расписание со стороны САФУ).\n" +
                   $"Вы можете проверить расписание здесь: {Utils.EndPoint}/?timetable&group={group}";
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