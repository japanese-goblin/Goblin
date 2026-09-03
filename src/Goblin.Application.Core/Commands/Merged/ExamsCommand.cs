using Microsoft.Extensions.Logging;

namespace Goblin.Application.Core.Commands.Merged;

public class ExamsCommand(IScheduleService scheduleService, ILogger<ExamsCommand> logger) : ITextCommand
{
    public bool IsAdminCommand => false;

    public string[] Aliases => ["экзамены", "экзы"];

    public async Task<CommandExecutionResult> Execute(Message msg, BotUser user)
    {
        if (!user.NarfuGroup.HasValue)
        {
            return CommandExecutionResult.Failed(DefaultErrors.GroupNotSet);
        }

        try
        {
            var result = await scheduleService.GetExams(user.NarfuGroup.Value);
            return CommandExecutionResult.Success(result.Message);
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
