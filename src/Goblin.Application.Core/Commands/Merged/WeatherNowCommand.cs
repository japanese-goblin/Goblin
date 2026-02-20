using Goblin.Application.Core.Abstractions;
using Goblin.Application.Core.Models;
using Goblin.Application.Core.Results.Failed;
using Goblin.Domain.Entities;

namespace Goblin.Application.Core.Commands.Merged;

public class WeatherNowCommand : IKeyboardCommand, ITextCommand
{
    public string Trigger => "weatherNow";

    public bool IsAdminCommand => false;
    public string[] Aliases => new[] { "погода" };
    private readonly IWeatherService _weatherService;

    public WeatherNowCommand(IWeatherService weatherService)
    {
        _weatherService = weatherService;
    }

    public async Task<IResult> Execute(Message msg, BotUser user)
    {
        if(!string.IsNullOrWhiteSpace(msg.Payload))
        {
            return await ExecutePayload(user);
        }

        return await ExecuteText(msg, user);
    }

    private async Task<IResult> ExecuteText(Message msg, BotUser user)
    {
        var city = msg.CommandParameters.FirstOrDefault();

        if(string.IsNullOrWhiteSpace(user.WeatherCity) && string.IsNullOrWhiteSpace(city))
        {
            return new FailedResult(DefaultErrors.CityNotSet);
        }

        if(!string.IsNullOrWhiteSpace(city))
        {
            return await _weatherService.GetCurrentWeather(city);
        }

        return await _weatherService.GetCurrentWeather(user.WeatherCity);
    }

    private async Task<IResult> ExecutePayload(BotUser user)
    {
        if(string.IsNullOrWhiteSpace(user.WeatherCity))
        {
            return new FailedResult(DefaultErrors.CityNotSet);
        }

        return await _weatherService.GetCurrentWeather(user.WeatherCity);
    }
}