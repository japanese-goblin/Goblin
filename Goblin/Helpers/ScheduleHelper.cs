using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using Goblin.Models;
using Calendar = Ical.Net.Calendar;

namespace Goblin.Helpers
{
    public static class ScheduleHelper
    {
        public static string GetSchedule(DateTime date, short usergroup)
        {
            var result = $"Расписание на {date:dd.MM}:\n";
            var res = GetSchedule(usergroup, out var lessons);
            if (!res)
            {
                return $"Какая-то ошибочка :с\n Возможно, сайт с расписанием недоступен, либо изменился номер группы на сайте.";
            }

            lessons = lessons.Where(x => x.Time.DayOfYear == date.DayOfYear).ToList();

            if (lessons.Count == 0) return $"На {date:dd.MM} расписание отсутствует!";

            foreach (var l in lessons.Where(x => x.Time.DayOfYear == date.DayOfYear))
            {
                result += $"{l.StartEndTime} - {l.Name} ({l.Type})\nУ группы {l.Groups}\n В аудитории {l.Address}\n\n";
            }

            return result;
        }

        public static bool GetSchedule(short usergroup, out List<Lesson> lessons)
        {
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

            var calendar = Calendar.Load(calen);
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
    }
}