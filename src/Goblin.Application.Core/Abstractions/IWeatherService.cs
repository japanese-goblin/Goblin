namespace Goblin.Application.Core.Abstractions;

public interface IWeatherService
{
    public Task<CommandExecutionResult> GetCurrentWeather(string city, CancellationToken ct = default);

    public Task<CommandExecutionResult> GetDailyWeather(string city, DateTime date, CancellationToken ct = default);
}