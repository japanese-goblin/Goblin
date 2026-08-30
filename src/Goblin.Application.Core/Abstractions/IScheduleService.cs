namespace Goblin.Application.Core.Abstractions;

public interface IScheduleService
{
    public Task<CommandExecutionResult> GetSchedule(int narfuGroup, DateTime date, CancellationToken ct = default);
    public Task<CommandExecutionResult> GetExams(int narfuGroup, CancellationToken ct = default);
}