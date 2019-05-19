using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Narfu;
using Narfu.Schedule;

namespace Goblin.WebUI.Controllers
{
    public class ScheduleController : Controller
    {
        private readonly NarfuService _service;

        public ScheduleController(NarfuService service)
        {
            _service = service;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Show(int id)
        {
            if(!ModelState.IsValid || !_service.Students.IsCorrectGroup(id))
            {
                HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return View("Error", $"Группа с номером {id} не найдена");
            }

            var group = _service.Students.GetGroupByRealId(id);
            var (error, _, lessons) = await _service.Students.GetSchedule(id);

            if(error)
            {
                HttpContext.Response.StatusCode = (int)HttpStatusCode.BadGateway;
                return View("Error", "Сайт с расписанием недоступен. Попробуйте позже.");
            }

            ViewBag.Title = $"{group.RealId} - {group.Name}";
            var result = lessons.GroupBy(x => StudentsSchedule.GetWeekNumber(x.StartTime))
                                .ToDictionary(x => $"{x.First().StartTime:dd.MM.yyyy} - {x.Last().StartTime:dd.MM.yyyy}",
                                              x => x.ToArray()); // TODO: fix key
            return View(result);
        }
    }
}