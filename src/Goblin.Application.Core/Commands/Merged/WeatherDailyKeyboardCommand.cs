using System.Threading.Tasks;
using Goblin.Application.Core.Abstractions;
using Goblin.Application.Core.Models;
using Goblin.Application.Core.Results.Failed;
using Goblin.Application.Core.Results.Success;
using Goblin.Domain.Abstractions;

namespace Goblin.Application.Core.Commands.Merged;

public class WeatherDailyKeyboardCommand : IKeyboardCommand, ITextCommand
{
    public string Trigger => "weatherDailyKeyboard";

    public bool IsAdminCommand => false;
    public string[] Aliases => new[] { "ежедневная" }; //TODO: lol

    public Task<IResult> Execute(Message msg, BotUser user)
    {
        if(string.IsNullOrWhiteSpace(user.WeatherCity))
        {
            return Task.FromResult<IResult>(new FailedResult(DefaultErrors.CityNotSet));
        }

        return Task.FromResult<IResult>(new SuccessfulResult
        {
            Message = "Выберите день для получения погоды:",
            Keyboard = DefaultKeyboards.GetDailyWeatherKeyboard()
        });
    }
}