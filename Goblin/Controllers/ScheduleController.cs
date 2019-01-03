using System.Linq;
using System.Threading.Tasks;
using Goblin.Schedule;
using Microsoft.AspNetCore.Mvc;

namespace Goblin.Controllers
{
    public class ScheduleController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Show(int id)
        {
            if (!ModelState.IsValid || !StudentsSchedule.IsCorrectGroup(id))
            {
                HttpContext.Response.StatusCode = 404;
                return View("Error");
            }

            var group = StudentsSchedule.GetGroupByRealId(id);
            ViewBag.Title = $"{group.RealId} - {group.Name}";
            var response = await StudentsSchedule.GetSchedule(id);
            if (!response.IsError)
            {
                var result = response.Lessons.GroupBy(x => StudentsSchedule.GetWeekNumber(x.Time))
                    .ToDictionary(x => $"{x.First().Time:dd.MM.yyyy} - {x.Last().Time:dd.MM.yyyy}",
                        x => x.ToList()); // TODO: fix key
                return View(result);
            }

            return View("Error");
        }
    }
}