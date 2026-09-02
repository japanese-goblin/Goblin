using Goblin.Narfu.Abstractions;
using Microsoft.Extensions.Logging;

namespace Goblin.Application.Core.Commands.Merged;

public class ExamsCommand(INarfuApi api, ILogger<ExamsCommand> logger) : ITextCommand
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
            var lessons = await api.Students.GetExams(user.NarfuGroup.Value);
            var str = lessons.ToString();
            if (str.Length > 4096)
            {
                str = $"{str[..4000]}...\n\nПолный список экзаменов можете посмотреть на сайте";
            }

            return CommandExecutionResult.Success(str);
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
