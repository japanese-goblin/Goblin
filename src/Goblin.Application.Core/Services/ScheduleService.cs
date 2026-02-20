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

    public async Task<CommandExecutionResult> GetSchedule(int narfuGroup, DateTime date)
    {
        var group = _narfuApi.Students.GetGroupByRealId(narfuGroup);
        if(group is null)
        {
            return CommandExecutionResult.Failed($"Группа {narfuGroup} не найдена");
        }

        try
        {
            var schedule = await _narfuApi.Students.GetScheduleAtDate(narfuGroup, date);

            return CommandExecutionResult.Success(schedule.ToString(), DefaultKeyboards.GetScheduleKeyboard());
        }
        catch(Exception ex) when(ex is HttpRequestException or TaskCanceledException)
        {
            return CommandExecutionResult.Failed(DefaultErrors.NarfuSiteIsUnavailable);
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "Ошибка при получении расписания на день");
            return CommandExecutionResult.Failed(DefaultErrors.NarfuUnexpectedError);
        }
    }
}