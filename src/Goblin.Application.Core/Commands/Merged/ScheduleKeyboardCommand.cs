namespace Goblin.Application.Core.Commands.Merged;

public class ScheduleKeyboardCommand : IKeyboardCommand, ITextCommand
{
    public string Trigger => "scheduleKeyboard";

    public bool IsAdminCommand => false;

    public string[] Aliases => ["расписание"];

    public Task<CommandExecutionResult> Execute(Message msg, BotUser user)
    {
        if(!user.NarfuGroup.HasValue)
        {
            return Task.FromResult(CommandExecutionResult.Failed(DefaultErrors.GroupNotSet));
        }

        return Task.FromResult(CommandExecutionResult.Success("Выберите день", DefaultKeyboards.GetScheduleKeyboard()));
    }
}