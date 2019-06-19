using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Goblin.WebUI.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Narfu;

namespace Goblin.WebUI.Controllers
{
    public class ScheduleController : Controller
    {
        private readonly NarfuService _service;
        private readonly ILogger<ScheduleController> _logger;

        public ScheduleController(NarfuService service, ILogger<ScheduleController> logger)
        {
            _service = service;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Show(int id) //TODO
        {
            if(!ModelState.IsValid || !_service.Students.IsCorrectGroup(id))
            {
                _logger.LogWarning("Группа с ID {0} не найдена", id);
                HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return View("Error", $"Группа с номером {id} не найдена");
            }

            var group = _service.Students.GetGroupByRealId(id);
            var (error, code, lessons) = await _service.Students.GetSchedule(id);

            if(error)
            {
                _logger.LogWarning("Ошибка получения расписания (Код - {0})", code);
                HttpContext.Response.StatusCode = (int)HttpStatusCode.BadGateway;
                return View("Error", $"Сайт с расписанием недоступен (Код ошибки - {code}). Попробуйте позже.");
            }

            ViewBag.HtmlTitle = $"{group.RealId} - {group.Name}";
            var result = lessons.GroupBy(x => x.StartTime.GetWeekNumber())
                                .ToDictionary(x => $"{x.First().StartTime:dd.MM.yyyy} - {x.Last().StartTime:dd.MM.yyyy}",
                                              x => x.ToArray()); // TODO: fix key
            return View(result);
        }
    }
}