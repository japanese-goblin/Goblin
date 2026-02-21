namespace Goblin.Application.Core.Commands.Keyboard;

public class ScheduleCommand(IScheduleService api) : IKeyboardCommand
{
    public string Trigger => "schedule";

    public async Task<CommandExecutionResult> Execute(Message msg, BotUser user)
    {
        if(!user.NarfuGroup.HasValue)
        {
            return CommandExecutionResult.Failed(DefaultErrors.GroupNotSet);
        }

        var date = msg.ParsedPayload[Trigger];
        return await api.GetSchedule(user.NarfuGroup.Value, DateTime.Parse(date));
    }
}