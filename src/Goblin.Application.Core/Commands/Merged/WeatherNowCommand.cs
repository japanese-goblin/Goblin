using System.Linq;
using System.Threading.Tasks;
using Goblin.Application.Core.Abstractions;
using Goblin.Application.Core.Results.Failed;
using Goblin.Domain.Abstractions;

namespace Goblin.Application.Core.Commands.Merged
{
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

        public async Task<IResult> Execute<T>(IMessage msg, BotUser user) where T : BotUser
        {
            if(!string.IsNullOrWhiteSpace(msg.MessagePayload))
            {
                return await ExecutePayload(user);
            }

            return await ExecuteText(msg, user);
        }

        private async Task<IResult> ExecuteText(IMessage msg, BotUser user)
        {
            var city = msg.MessageParams.FirstOrDefault();

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
}