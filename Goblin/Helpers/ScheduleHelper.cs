using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Goblin.Models;
using Newtonsoft.Json;
using Calendar = Ical.Net.Calendar;

namespace Goblin.Helpers
{
    public static class ScheduleHelper
    {
        public static List<Group> Groups = new List<Group>();

        static ScheduleHelper()
        {
            Groups = JsonConvert.DeserializeObject<List<Group>>(File.ReadAllText("Groups.json"));
        }

        public static string GetScheduleAtDate(DateTime date, int usergroup)
        {
            var result = $"Расписание на {date:dd.MM}:\n";
            var res = GetSchedule(usergroup, out var lessons);
            if (!res)
            {
                var group = GetGroupByRealId(usergroup).SiteId;
                return $"Какая-то ошибочка :с\n" +
                       $"Возможно, сайт с расписанием недоступен, либо изменился номер группы на сайте.\n" +
                       $"Вы можете проверить расписание здесь: http://ruz.narfu.ru/?icalendar&oid={group}&from={DateTime.Now:dd.MM.yyyy}";
            }

            lessons = lessons.Where(x => x.Time.DayOfYear == date.DayOfYear).ToList();

            if (lessons.Count == 0) return $"На {date:dd.MM} расписание отсутствует!";

            foreach (var lesson in lessons.Where(x => x.Time.DayOfYear == date.DayOfYear))
            {
                result += $"{lesson.StartEndTime} - {lesson.Name} [{lesson.Type}] ({lesson.Teacher})\n" +
                          $"У группы {lesson.Groups}\n" +
                          $"В аудитории {lesson.Auditory}\n\n";
            }

            return result;
        }

        public static bool GetSchedule(int realGroup, out List<Lesson> lessons)
        {
            var usergroup = GetGroupByRealId(realGroup).SiteId;
            lessons = new List<Lesson>();
            string calen;
            using (var client = new WebClient())
            {
                try
                {
                    client.Encoding = Encoding.UTF8;
                    calen = client.DownloadString(
                        $"http://ruz.narfu.ru/?icalendar&oid={usergroup}&from={DateTime.Now:dd.MM.yyyy}");
                }
                catch (WebException)
                {
                    return false;
                }
            }

            Calendar calendar;
            try
            {
                calendar = Calendar.Load(calen);
            }
            catch
            {
                return false;
            }
            var events = calendar.Events.Distinct().OrderBy(x => x.Start.Value).ToList();
            if (!events.Any())
            {
                return true;
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
                    Number = (byte)a[0].ElementAt(0)
                };
                lessons.Add(les);
            }

            return true;
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