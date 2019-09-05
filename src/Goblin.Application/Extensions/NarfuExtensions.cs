using System;
using System.Threading.Tasks;
using Flurl.Http;
using Goblin.Application.Abstractions;
using Goblin.Application.Results.Failed;
using Goblin.Application.Results.Success;
using Goblin.Narfu.Schedule;

namespace Goblin.Application.Extensions
{
    public static class NarfuExtensions
    {
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
            catch(FlurlHttpException ex)
            {
                return new FailedResult($"Невозможно получить экзамены с сайта. Попробуйте позже." +
                                        $" (Код ошибки - {ex.Call.HttpStatus})");
            }
            catch(Exception ex)
            {
                return new FailedResult($"Непредвиденная ошибка получения экзаменов с сайта ({ex.Message}). Попробуйте позже.");
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
            catch(FlurlHttpException ex)
            {
                return new FailedResult($"Невозможно получить расписание с сайта. Попробуйте позже." +
                                        $" (Код ошибки - {ex.Call.HttpStatus})");
            }
            catch(Exception ex)
            {
                return new FailedResult($"Непредвиденная ошибка получения расписания с сайта ({ex.Message}). Попробуйте позже.");
            }
        }
    }
}