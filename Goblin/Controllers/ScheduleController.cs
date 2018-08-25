using Goblin.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using Goblin.Helpers;
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
            ScheduleHelper.GetSchedule(id, out var lessons);
            var result = lessons.GroupBy(x => ScheduleHelper.GetWeekNumber(x.Time))
                .ToDictionary(x => $"{x.First().Time:d} - {x.Last().Time:d}", x => x.ToList()); // TODO: fix key
            return View(result);
        }
    }
}