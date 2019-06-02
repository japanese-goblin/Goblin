using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Goblin.WebUI.Extensions;
using Microsoft.AspNetCore.Mvc;
using Narfu;

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

        public async Task<IActionResult> Show(int id) //TODO
        {
            if(!ModelState.IsValid || !_service.Students.IsCorrectGroup(id))
            {
                HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return View("Error", $"Группа с номером {id} не найдена");
            }

            var group = _service.Students.GetGroupByRealId(id);
            var (error, code, lessons) = await _service.Students.GetSchedule(id);

            if(error)
            {
                HttpContext.Response.StatusCode = (int)HttpStatusCode.BadGateway;
                return View("Error", $"Сайт с расписанием недоступен (Код ошибки - {code}). Попробуйте позже.");
            }

            ViewBag.HtmlTitle = $"{group.RealId} - {group.Name}";
            var result = lessons.GroupBy(x => x.StartTime.GetWeekNumber())
                                .ToDictionary(x =>$"{x.First().StartTime:dd.MM.yyyy} - {x.Last().StartTime:dd.MM.yyyy}",
                                              x => x.ToArray()); // TODO: fix key
            return View(result);
        }
    }
}