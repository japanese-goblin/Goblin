using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
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

        static StudentsSchedule()
        {
            var path = Path.GetDirectoryName(Assembly.GetAssembly(typeof(StudentsSchedule)).Location); //TODO:
            Groups = JsonConvert.DeserializeObject<Group[]>(File.ReadAllText($"{path}/Data/Groups.json"));
        }

        public static async Task<(bool Error, HttpStatusCode Code, Lesson[] Lessons)> GetSchedule(int groupId)
        {
            var siteGroup = GetGroupByRealId(groupId).SiteId;
            var requestUrl = $"?icalendar&oid={siteGroup}&cod={groupId}&from={DateTime.Now:dd.MM.yyyy}";

            HttpResponseMessage response;
            try
            {
                response = await Utils.Client.GetAsync(requestUrl);
            }
            catch(TaskCanceledException ex)
            {
                return (true, HttpStatusCode.GatewayTimeout, null);
            }

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
                    Number = (byte) description[0].ElementAt(0),
                    Groups = description[1].Substring(3),
                    Name = description[2],
                    Type = description[3],
                    Teacher = description[4],
                    StartTime = ev.DtStart.AsSystemLocal,
                    EndTime = ev.DtEnd.AsSystemLocal,
                    StartEndTime = description[0].Replace(")", "").Replace("(", "").Replace("п", ")")
                };
            }).ToArray();

            return (false, response.StatusCode, lessons);
        }

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

        public static async Task<string> GetExams(int realGroup)
        {
            var res = await GetSchedule(realGroup);

            if(res.Error)
            {
                var group = GetGroupByRealId(realGroup).SiteId;
                return GenerateErrorMessage(res.Code, group);
            }

            var lessons = res
                          .Lessons
                          .Where(x => x.Type.ToLower().Contains("экзамен") || x.Type.ToLower().Contains("зачет"))
                          .OrderBy(x => x.StartTime).ToArray();

            if(lessons.Length == 0)
            {
                return "На данный момент список экзаменов отсутствует";
            }

            var strBuilder = new StringBuilder();

            strBuilder.AppendLine("Список экзаменов:");
            foreach(var group in lessons.GroupBy(x => x.Name))
            {
                var first = group.First();
                var last = group.Last();
                strBuilder.AppendFormat("{0:dd.MM.yyyy (dddd)} ({1:HH:mm} - {2:HH:mm}) - {3} [{4}] ({5})",
                                        last.StartTime, first.StartTime, last.EndTime, last.Name, last.Type, last.Teacher).AppendLine();
                strBuilder.AppendFormat("У группы {0}", last.Groups).AppendLine();
                strBuilder.AppendFormat("В аудитории {0}", last.Auditory).AppendLine();
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
                   "Сайт с расписанием недоступен (либо сломалось расписание со стороны САФУ).\n" +
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