using Goblin.Helpers;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

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
            if (!ModelState.IsValid) return View("Error");

            var group = ScheduleHelper.GetGroupByRealId(id);
            ViewBag.Title = $"{group.RealId} - {group.Name}";
            var response = await ScheduleHelper.GetSchedule(id);
            if (!response.IsError)
            {
                var result = response.Lessons.GroupBy(x => ScheduleHelper.GetWeekNumber(x.Time))
                             .ToDictionary(x => $"{x.First().Time:dd.MM.yyyy} - {x.Last().Time:dd.MM.yyyy}",
                                           x => x.ToList()); // TODO: fix key
                return View(result);
            }
            else
            {
                return View("Error");
            }
        }
    }
}