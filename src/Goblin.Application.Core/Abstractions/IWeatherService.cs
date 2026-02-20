namespace Goblin.Application.Core.Abstractions;

public interface IWeatherService
{
    public Task<CommandExecutionResult> GetCurrentWeather(string city);

    public Task<CommandExecutionResult> GetDailyWeather(string city, DateTime date);
}