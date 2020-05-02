using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Flurl.Http;
using Goblin.Narfu;
using Goblin.Narfu.Models;
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

        [ResponseCache(Duration = 600, Location = ResponseCacheLocation.Client)]
        public async Task<IActionResult> Show(int id, DateTime date)
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
                var lessons = (await _narfuApi.Students.GetSchedule(group.RealId, date)).ToList();

                var scheduleLink = _narfuApi.Students.GenerateScheduleLink(group.RealId);
                var webcalLink = _narfuApi.Students.GenerateScheduleLink(group.RealId, true);

                return View(new LessonsViewModel
                {
                    Lessons = MagicWithLessons(lessons),
                    GroupTitle = $"{group.RealId} - {group.Name}",
                    ScheduleLink = scheduleLink,
                    WebcalLink = webcalLink
                });
            }
            catch(FlurlHttpException)
            {
                return View("Error", new ErrorViewModel
                {
                    Description = "Сайт с расписанием временно недоступен. Попробуйте позже."
                });
            }
            catch(Exception)
            {
                return View("Error", new ErrorViewModel
                {
                    Description = "Непредвиденная ошибка при получении расписания. Попробуйте позже."
                });
            }
        }

        // какой ужас.... но работает (TODO)
        [NonAction]
        private static Dictionary<string, Lesson[]> MagicWithLessons(List<Lesson> lessons)
        {
            var first = lessons.First().StartTime.Date;
            var last = lessons.Last().StartTime.Date;
            var dif = (last - first).Days;
            for(var i = 0; i <= dif; i++)
            {
                var day = first.AddDays(i);
                if(day.DayOfWeek == DayOfWeek.Sunday) // по воскресениям пар нет
                {
                    continue;
                }

                var lessonsAtDay = lessons.Where(x => x.StartTime.Date == day).ToArray();
                if(lessonsAtDay.Any())
                {
                    var max = lessonsAtDay.Max(x => x.Number); // количество пар в этот день
                    for(var lessonNumber = 1; lessonNumber <= max; lessonNumber++)
                    {
                        if(!lessonsAtDay.Any(x => x.Number == lessonNumber)) // если нет пары под этим номером 
                        {
                            lessons.Add(new Lesson
                            {
                                StartTime = day,
                                Number = lessonNumber
                            });
                        }
                    }
                }

                // добавление пустого дня в расписании
                lessons.Add(new Lesson
                {
                    StartTime = day,
                    Number = 0
                });
            }

            var dict = lessons.OrderBy(x => x.StartTime.Date)
                              .ThenBy(x => x.Number)
                              .GroupBy(x => x.StartTime.GetStartOfWeek());
            return dict.ToDictionary(x =>
                                     {
                                         var start = x.First().StartTime.GetStartOfWeek();
                                         return
                                                 $"{start:dd.MM.yyyy} - {start.AddDays(5):dd.MM.yyyy}";
                                     },
                                     x => x.ToArray());
        }
    }
}