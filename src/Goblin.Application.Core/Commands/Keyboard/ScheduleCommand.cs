namespace Goblin.Application.Core.Commands.Keyboard;

public class ScheduleCommand : IKeyboardCommand
{
    public string Trigger => "schedule";

    private readonly IScheduleService _api;

    public ScheduleCommand(IScheduleService api)
    {
        _api = api;
    }

    public async Task<CommandExecutionResult> Execute(Message msg, BotUser user)
    {
        if(user.NarfuGroup == 0)
        {
            return CommandExecutionResult.Failed(DefaultErrors.GroupNotSet);
        }

        var date = msg.ParsedPayload[Trigger];
        return await _api.GetSchedule(user.NarfuGroup, DateTime.Parse(date));
    }
}