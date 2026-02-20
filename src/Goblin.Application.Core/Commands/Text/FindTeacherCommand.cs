using Goblin.Narfu.Abstractions;
using Microsoft.Extensions.Logging;

namespace Goblin.Application.Core.Commands.Text;

public class FindTeacherCommand : ITextCommand
{
    public bool IsAdminCommand => false;
    public string[] Aliases => ["препод"];
    private readonly INarfuApi _narfuApi;
    private readonly ILogger<FindTeacherCommand> _logger;

    public FindTeacherCommand(INarfuApi narfuApi, ILogger<FindTeacherCommand> logger)
    {
        _narfuApi = narfuApi;
        _logger = logger;
    }

    public async Task<CommandExecutionResult> Execute(Message msg, BotUser user)
    {
        var teacherName = string.Join(' ', msg.CommandParameters);
        if(string.IsNullOrWhiteSpace(teacherName))
        {
            return CommandExecutionResult.Failed("Укажите имя и фамилию преподавателя.");
        }

        try
        {
            var findResult = await _narfuApi.Teachers.FindByName(teacherName);
            if(findResult.Length == 0)
            {
                return CommandExecutionResult.Failed("Преподаватель с такими данными не найден.");
            }

            if(findResult.Length > 9)
            {
                return CommandExecutionResult.Failed("Найдено слишком много преподавателей. Укажите более точные данные.");
            }

            var keyboard = new CoreKeyboard
            {
                IsInline = true
            };
            foreach(var teacher in findResult)
            {
                keyboard.AddButton(teacher.Name, CoreKeyboardButtonColor.Primary,
                                   "teacherSchedule", teacher.Id.ToString());
                keyboard.AddLine();
            }

            keyboard.AddReturnToMenuButton(false);
            return CommandExecutionResult.Success("Выберите преподавателя из списка:", keyboard);
        }
        catch(Exception ex) when(ex is HttpRequestException or TaskCanceledException)
        {
            return CommandExecutionResult.Failed(DefaultErrors.NarfuSiteIsUnavailable);
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "Ошибка при поиске преподавателя");
            return CommandExecutionResult.Failed(DefaultErrors.NarfuUnexpectedError);
        }
    }
}