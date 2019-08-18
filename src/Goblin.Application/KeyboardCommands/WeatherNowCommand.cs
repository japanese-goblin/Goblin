using System.Collections.Generic;
using System.Threading.Tasks;
using Goblin.Application.Abstractions;
using Goblin.Application.Results;
using Goblin.Domain.Entities;
using Goblin.OpenWeatherMap;
using VkNet.Model;

namespace Goblin.Application.KeyboardCommands
{
    public class WeatherNowCommand : IKeyboardCommand
    {
        public string Trigger => "weatherNow";
        
        private readonly OpenWeatherMapApi _api;

        public WeatherNowCommand(OpenWeatherMapApi api)
        {
            _api = api;
        }
        
        public async Task<IResult> Execute(Message msg, BotUser user)
        {
            if(string.IsNullOrWhiteSpace(user.WeatherCity))
            {
                return new FailedResult("Для получения погоды сначала необходимо установить город.");
            }

            try
            {
                var weather = await _api.GetCurrentWeather(user.WeatherCity);
                return new SuccessfulResult
                {
                    Message = weather.ToString()
                };
            }
            catch
            {
                return new FailedResult("Невозможно получить погоду с внешнего сайта. Попробуйте позже.");
            }
        }
    }
}