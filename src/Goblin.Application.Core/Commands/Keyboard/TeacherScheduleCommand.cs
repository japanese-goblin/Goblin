using Goblin.Narfu.Abstractions;

namespace Goblin.Application.Core.Commands.Keyboard;

public class TeacherScheduleCommand : IKeyboardCommand
{
    public string Trigger => "teacherSchedule";

    private readonly INarfuApi _narfuApi;

    public TeacherScheduleCommand(INarfuApi narfuApi)
    {
        _narfuApi = narfuApi;
    }

    public async Task<CommandExecutionResult> Execute(Message msg, BotUser user)
    {
        var dict = msg.ParsedPayload;
        var isExists = dict.TryGetValue(Trigger, out var idString);
        if(!isExists)
        {
            return CommandExecutionResult.Failed("Невозожно получить ID преподавателя.");
        }

        var isCorrectId = int.TryParse(idString, out var id);
        if(!isCorrectId)
        {
            return CommandExecutionResult.Failed("Некорректный ID преподавателя.");
        }

        try
        {
            var schedule = await _narfuApi.Teachers.GetLimitedSchedule(id);
            return CommandExecutionResult.Success(schedule.ToString());
        }
        catch(Exception ex) when(ex is HttpRequestException or TaskCanceledException)
        {
            return CommandExecutionResult.Failed(DefaultErrors.NarfuSiteIsUnavailable);
        }
        catch(Exception)
        {
            return CommandExecutionResult.Failed(DefaultErrors.NarfuUnexpectedError);
        }
    }
}