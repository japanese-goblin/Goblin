namespace Goblin.Application.Core.Abstractions;

public interface IScheduleService
{
    public Task<CommandExecutionResult> GetSchedule(int narfuGroup, DateTime date);
}