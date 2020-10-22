using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Flurl.Http;
using Goblin.Narfu.Abstractions;
using Goblin.Narfu.Models;
using Goblin.WebApp.Extensions;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Serilog;

namespace Goblin.WebApp.Pages.Schedule
{
    public class Show : PageModel
    {
        public Dictionary<string, Lesson[]> Lessons { get; set; }
        public string GroupTitle { get; set; }
        public string ScheduleLink { get; set; }
        public string WebcalLink { get; set; }

        public int RealGroupId { get; set; }

        public string ErrorMessage { get; set; }
        private readonly INarfuApi _narfuApi;

        public Show(INarfuApi narfuApi)
        {
            _narfuApi = narfuApi;
        }

        public async Task OnGet(int id, DateTime date)
        {
            RealGroupId = id;
            if(!_narfuApi.Students.IsCorrectGroup(id))
            {
                Log.Warning("Группа с id {0} не найдена", id);
                HttpContext.Response.StatusCode = (int) HttpStatusCode.BadRequest;
                ErrorMessage = $"Группа с номером {id} не найдена";
            }

            var group = _narfuApi.Students.GetGroupByRealId(id);

            try
            {
                var lessons = await _narfuApi.Students.GetSchedule(group.RealId, date);

                Lessons = MagicWithLessons(lessons.ToList());
                GroupTitle = $"{group.RealId} - {group.Name}";
                ScheduleLink = _narfuApi.Students.GenerateScheduleLink(group.RealId);
                WebcalLink = _narfuApi.Students.GenerateScheduleLink(group.RealId, true);
            }
            catch(FlurlHttpException)
            {
                ErrorMessage = "Сайт с расписанием временно недоступен. Попробуйте позже.";
            }
            catch(Exception)
            {
                ErrorMessage = "Непредвиденная ошибка при получении расписания. Попробуйте позже.";
            }
        }

        // какой ужас.... но работает (TODO)
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
                        if(lessonsAtDay.All(x => x.Number != lessonNumber)) // если нет пары под этим номером 
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