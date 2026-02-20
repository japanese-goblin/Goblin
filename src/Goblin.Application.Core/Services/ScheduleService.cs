using Goblin.Application.Core.Abstractions;
using Goblin.Application.Core.Results.Failed;
using Goblin.Application.Core.Results.Success;
using Goblin.Narfu.Abstractions;
using Microsoft.Extensions.Logging;

namespace Goblin.Application.Core.Services;

public class ScheduleService : IScheduleService
{
    private readonly INarfuApi _narfuApi;
    private readonly ILogger<ScheduleService> _logger;

    public ScheduleService(INarfuApi narfuApi, ILogger<ScheduleService> logger)
    {
        _narfuApi = narfuApi;
        _logger = logger;
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
        catch(Exception ex) when(ex is HttpRequestException or TaskCanceledException)
        {
            return new FailedResult(DefaultErrors.NarfuSiteIsUnavailable);
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "Ошибка при получении расписания на день");
            return new FailedResult(DefaultErrors.NarfuUnexpectedError);
        }
    }
}