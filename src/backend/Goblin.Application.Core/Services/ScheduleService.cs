using System;
using System.Net.Http;
using System.Threading.Tasks;
using Goblin.Application.Core.Abstractions;
using Goblin.Application.Core.Results.Failed;
using Goblin.Application.Core.Results.Success;
using Goblin.Narfu;
using Goblin.Narfu.Abstractions;
using Serilog;

namespace Goblin.Application.Core.Services;

public class ScheduleService : IScheduleService
{
    private readonly INarfuApi _narfuApi;

    public ScheduleService(INarfuApi narfuApi)
    {
        _narfuApi = narfuApi;
    }

    public async Task<IResult> GetSchedule(int narfuGroup, DateTime date)
    {
        if(!_narfuApi.Students.IsCorrectGroup(narfuGroup))
        {
            return new FailedResult($"Группа {narfuGroup} не найдена");
        }

        try
        {
            var schedule = await _narfuApi.Students.GetScheduleAtDate(narfuGroup, date);

            return new SuccessfulResult
            {
                Message = schedule.ToString(),
                Keyboard = DefaultKeyboards.GetScheduleKeyboard()
            };
        }
        catch(HttpRequestException)
        {
            return new FailedResult(DefaultErrors.NarfuSiteIsUnavailable);
        }
        catch(Exception ex)
        {
            Log.ForContext<NarfuApi>().Fatal(ex, "Ошибка при получении расписания на день");
            return new FailedResult(DefaultErrors.NarfuUnexpectedError);
        }
    }
}