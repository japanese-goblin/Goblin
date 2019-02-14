using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Narfu;

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
            if(!ModelState.IsValid || !StudentsSchedule.IsCorrectGroup(id))
            {
                HttpContext.Response.StatusCode = 404;
                return View("Error");
            }

            var group = StudentsSchedule.GetGroupByRealId(id);
            ViewBag.Title = $"{group.RealId} - {group.Name}";
            var (error, lessons) = await StudentsSchedule.GetSchedule(id);

            if(!error)
            {
                var result = lessons.GroupBy(x => StudentsSchedule.GetWeekNumber(x.Time))
                                    .ToDictionary(x => $"{x.First().Time:dd.MM.yyyy} - {x.Last().Time:dd.MM.yyyy}",
                                                  x => x.ToArray()); // TODO: fix key
                return View(result);
            }

            return View("Error");
        }
    }
}