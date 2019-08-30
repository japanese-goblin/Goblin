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

        [ResponseCache(Duration = 3600, Location = ResponseCacheLocation.Client)]
        public IActionResult Index()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
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
                var dict = lessons.GroupBy(x => x.StartTime.GetWeekNumber());
                
                return View(new LessonsViewModel
                {
                    Lessons = dict.ToDictionary(x =>
                                                {
                                                    var start = x.First().StartTime.GetStartOfWeek();
                                                    return
                                                            $"{start:dd.MM.yyyy} - {start.AddDays(5):dd.MM.yyyy}";
                                                },
                                                x => x.ToArray()),
                    GroupTitle = $"{group.RealId} - {group.Name}"
                });
            }
            catch(FlurlHttpException ex)
            {
                Log.Error(ex, "ruz.narfu.ru недоступен (http code - {0})", ex.Call.HttpStatus);
                return View("Error", new ErrorViewModel
                {
                    Description = $"Сайт с расписанием временно недоступен (Код ошибки - {ex.Call.HttpStatus}). Попробуйте позже."
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