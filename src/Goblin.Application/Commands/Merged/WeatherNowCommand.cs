using System.Linq;
using System.Threading.Tasks;
using Goblin.Application.Abstractions;
using Goblin.Application.Extensions;
using Goblin.Application.Results.Failed;
using Goblin.Domain.Entities;
using Goblin.OpenWeatherMap;
using VkNet.Model;

namespace Goblin.Application.Commands.Merged
{
    public class WeatherNowCommand : IKeyboardCommand, ITextCommand
    {
        public string Trigger => "weatherNow";

        public bool IsAdminCommand => false;
        public string[] Aliases => new[] { "погода" };

        private readonly OpenWeatherMapApi _api;

        public WeatherNowCommand(OpenWeatherMapApi api)
        {
            _api = api;
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
            var city = msg.GetCommandParameters().FirstOrDefault();
            if(!string.IsNullOrWhiteSpace(city))
            {
                return await _api.GetCurrentWeatherWithResult(city);
            }

            if(string.IsNullOrWhiteSpace(user.WeatherCity) && string.IsNullOrWhiteSpace(city))
            {
                return new FailedResult("Для получения погоды сначала необходимо установить город.");
            }

            if(!string.IsNullOrWhiteSpace(city))
            {
                return await _api.GetCurrentWeatherWithResult(city);
            }

            return await _api.GetCurrentWeatherWithResult(user.WeatherCity);
        }

        private async Task<IResult> ExecutePayload(BotUser user)
        {
            if(string.IsNullOrWhiteSpace(user.WeatherCity))
            {
                return new FailedResult("Для получения погоды сначала необходимо установить город.");
            }

            return await _api.GetCurrentWeatherWithResult(user.WeatherCity);
        }
    }
}