using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Flurl.Http;
using Goblin.Narfu;
using Goblin.WebApp.Extensions;
using Goblin.WebApp.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace Goblin.WebApp.Controllers
{
    public class ScheduleController : Controller
    {
        private readonly NarfuApi _narfuApi;

        public ScheduleController(NarfuApi narfuApi)
        {
            _narfuApi = narfuApi;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Show(int id)
        {
            if(!_narfuApi.Students.IsCorrectGroup(id))
            {
                Log.Warning("Группа с id {0} не найдена", id);
                HttpContext.Response.StatusCode = (int) HttpStatusCode.BadRequest;
                return View("Error", new ErrorViewModel
                {
                    Description = $"Группа с номером {id} не найдена"
                });
            }

            var group = _narfuApi.Students.GetGroupByRealId(id);

            try
            {
                var lessons = await _narfuApi.Students.GetSchedule(group.RealId);
                var dict = lessons.GroupBy(x => x.StartTime.GetWeekNumber())
                                  .ToDictionary(x => $"{x.First().StartTime:dd.MM.yyyy} - {x.Last().StartTime:dd.MM.yyyy}",
                                                x => x.ToArray());
                return View(new LessonsViewModel
                {
                    Lessons = dict,
                    GroupTitle = $"{group.RealId} - {group.Name}"
                });
            }
            catch(FlurlHttpException ex)
            {
                Log.Error("ruz.narfu.ru недоступен (http code - {0})", ex.Call.HttpStatus);
                return View("Error", new ErrorViewModel
                {
                    Description = $"Сайт с расписанием вернул ошибку ({ex.Call.HttpStatus}). Попробуйте позже."
                });
            }
            catch(Exception ex)
            {
                Log.Fatal(ex, "Ошибка при получении расписания");
                return View("Error", new ErrorViewModel
                {
                    Description = "Непрведиденная ошибка при получении расписания. Попробуйте позже."
                });
            }
        }
    }
}