using System;
using System.Threading.Tasks;
using Flurl.Http;
using Goblin.Application.Abstractions;
using Goblin.Application.Results.Failed;
using Goblin.Application.Results.Success;
using Goblin.Narfu;
using Goblin.Narfu.Schedule;
using Serilog;

namespace Goblin.Application.Extensions
{
    public static class NarfuExtensions
    {
        private const string SiteIsUnavailable = "Сайт с расписанием временно недоступен. Попробуйте позже.";

        public static async Task<IResult> GetExamsWithResult(this StudentsSchedule students, int group)
        {
            try
            {
                var lessons = await students.GetExams(group);
                return new SuccessfulResult
                {
                    Message = lessons.ToString()
                };
            }
            catch(FlurlHttpException)
            {
                return new FailedResult(SiteIsUnavailable);
            }
            catch(Exception ex)
            {
                Log.ForContext<NarfuApi>().Fatal(ex, "Ошибка при получении экзаменов");
                return new FailedResult("Непредвиденная ошибка получения экзаменов с сайта. Попробуйте позже.");
            }
        }

        public static async Task<IResult> GetScheduleAtDateWithResult(this StudentsSchedule students, int group, DateTime date)
        {
            try
            {
                var schedule = await students.GetScheduleAtDate(group, date);
                return new SuccessfulResult
                {
                    Message = schedule.ToString()
                };
            }
            catch(FlurlHttpException)
            {
                return new FailedResult(SiteIsUnavailable);
            }
            catch(Exception ex)
            {
                Log.ForContext<NarfuApi>().Fatal(ex, "Ошибка при получении расписания на день");
                return new FailedResult("Непредвиденная ошибка получения расписания с сайта. Попробуйте позже.");
            }
        }
    }
}