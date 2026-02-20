using Goblin.Application.Core.Abstractions;
using Goblin.Application.Core.Models;
using Goblin.Application.Core.Results.Failed;
using Goblin.Domain.Entities;

namespace Goblin.Application.Core.Commands.Keyboard;

public class ScheduleCommand : IKeyboardCommand
{
    public string Trigger => "schedule";
    private readonly IScheduleService _api;

    public ScheduleCommand(IScheduleService api)
    {
        _api = api;
    }

    public async Task<IResult> Execute(Message msg, BotUser user)
    {
        if(user.NarfuGroup == 0)
        {
            return new FailedResult(DefaultErrors.GroupNotSet);
        }

        var date = msg.ParsedPayload[Trigger];
        return await _api.GetSchedule(user.NarfuGroup, DateTime.Parse(date));
    }
}