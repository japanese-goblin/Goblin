using System.Linq;
using System.Threading.Tasks;
using Goblin.Application.Core.Abstractions;
using Goblin.Application.Core.Results.Failed;
using Goblin.Application.Core.Results.Success;
using Goblin.Domain.Entities;
using Goblin.OpenWeatherMap;

namespace Goblin.Application.Core.Commands.Merged
{
    public class WeatherNowCommand : IKeyboardCommand, ITextCommand
    {
        public bool IsAdminCommand => false;
        public string[] Aliases => new[] { "погода" };

        public string Trigger => "weatherNow";

        private readonly OpenWeatherMapApi _api;

        public WeatherNowCommand(OpenWeatherMapApi api)
        {
            _api = api;
        }

        public async Task<IResult> Execute(IMessage msg, BotUser user)
        {
            if(!string.IsNullOrWhiteSpace(msg.Payload))
            {
                return await ExecutePayload(user);
            }

            return await ExecuteText(msg, user);
        }

        private async Task<IResult> ExecuteText(IMessage msg, BotUser user)
        {
            var city = msg.MessageParams.FirstOrDefault();
            if(!string.IsNullOrWhiteSpace(city))
            {
                return new SuccessfulResult
                {
                    Message = (await _api.GetCurrentWeather(city)).ToString()
                };
            }

            if(string.IsNullOrWhiteSpace(user.WeatherCity) && string.IsNullOrWhiteSpace(city))
            {
                return new FailedResult(DefaultErrors.CityNotSet);
            }

            if(!string.IsNullOrWhiteSpace(city))
            {
                return new SuccessfulResult
                {
                    Message = (await _api.GetCurrentWeather(city)).ToString()
                };
            }

            return new SuccessfulResult
            {
                Message = (await _api.GetCurrentWeather(user.WeatherCity)).ToString()
            };
        }

        private async Task<IResult> ExecutePayload(BotUser user)
        {
            if(string.IsNullOrWhiteSpace(user.WeatherCity))
            {
                return new FailedResult(DefaultErrors.CityNotSet);
            }

            return new SuccessfulResult
            {
                Message = (await _api.GetCurrentWeather(user.WeatherCity)).ToString()
            };
        }
    }
}