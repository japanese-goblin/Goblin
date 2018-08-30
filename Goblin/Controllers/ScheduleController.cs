using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Goblin.Helpers;

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
            if (!ModelState.IsValid) return View("Error");

            ScheduleHelper.GetSchedule(id, out var lessons);
            var result = lessons.GroupBy(x => ScheduleHelper.GetWeekNumber(x.Time))
                         .ToDictionary(x => $"{x.First().Time:dd.MM.yyyy} - {x.Last().Time:dd.MM.yyyy}",
                                       x => x.ToList()); // TODO: fix key
            return View(result);
        }
    }
}