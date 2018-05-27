using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using Goblin.Models;
using Microsoft.AspNetCore.Mvc;
using Calendar = Ical.Net.Calendar;

namespace Goblin.Controllers
{
    public class ScheduleController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Show(short id)
        {
            ViewBag.Lessons = GetSchedule(id).GroupBy(x => $"{x.Time:dd-MM}");
            return View();
        }

        [NonAction]
        private List<Lesson> GetSchedule(short usergroup)
        {
            List<Lesson> Lessons = new List<Lesson>();
            string calen;
            using (var client = new WebClient())
            {
                try
                {
                    client.Encoding = Encoding.UTF8;
                    calen = client.DownloadString(
                        $"http://ruz.narfu.ru/?icalendar&oid={usergroup}&from={DateTime.Now:dd.MM.yyyy}");
                }
                catch (WebException e)
                {
                    return Lessons;
                }
            }

            var calendar = Calendar.Load(calen);
            var events = calendar.Events.Distinct().OrderBy(x => x.Start.Value).ToList();
            if (!events.Any()) return Lessons;
            foreach (var ev in events)
            {
                var a = ev.Description.Split('\n');
	            
                var les = new Lesson()
                {
                    Address = ev.Location,
                    Groups = a[1].Substring(3),
                    Name = a[2],
                    Teacher = a[4],
                    Time = ev.Start.AsSystemLocal,
                    Type = a[3]
                };
                Lessons.Add(les);

            }

            return Lessons;
        }
    }
}