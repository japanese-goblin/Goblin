using Goblin.Narfu.Abstractions;
using Microsoft.Extensions.Logging;

namespace Goblin.Application.Core.Services;

public class ScheduleService(INarfuApi narfuApi, ILogger<ScheduleService> logger) : IScheduleService
{
    public async Task<CommandExecutionResult> GetSchedule(int narfuGroup, DateTime date)
    {
        var group = narfuApi.Students.GetGroupByRealId(narfuGroup);
        if(group is null)
        {
            return CommandExecutionResult.Failed($"Группа {narfuGroup} не найдена");
        }

        try
        {
            var schedule = await narfuApi.Students.GetScheduleAtDate(narfuGroup, date);
            return CommandExecutionResult.Success(schedule.ToString(), DefaultKeyboards.GetScheduleKeyboard());
        }
        catch(Exception ex) when(ex is HttpRequestException or TaskCanceledException)
        {
            return CommandExecutionResult.Failed(DefaultErrors.NarfuSiteIsUnavailable);
        }
        catch(Exception ex)
        {
            logger.LogError(ex, "Ошибка при получении расписания на день");
            return CommandExecutionResult.Failed(DefaultErrors.NarfuUnexpectedError);
        }
    }
}