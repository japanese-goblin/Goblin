using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Goblin.Application.Abstractions;
using Goblin.Application.Results;
using Goblin.Domain.Entities;
using Goblin.OpenWeatherMap;
using Newtonsoft.Json;
using VkNet.Model;

namespace Goblin.Application.KeyboardCommands
{
    public class WeatherDailyCommand : IKeyboardCommand
    {
        public string Trigger => "weather";
        
        private readonly OpenWeatherMapApi _api;

        public WeatherDailyCommand(OpenWeatherMapApi api)
        {
            _api = api;
        }
        
        public async Task<IResult> Execute(Message msg, BotUser user = null)
        {
            if(string.IsNullOrWhiteSpace(user.WeatherCity))
            {
                return new FailedResult(new List<string>
                {
                    "Для получения погоды сначала необходимо установить город."
                });
            }

            var day = JsonConvert.DeserializeObject<Dictionary<string, string>>(msg.Payload)["weather"];

            try
            {
                var weather = await _api.GetDailyWeatherAt(user.WeatherCity, DateTime.Parse(day));
                return new SuccessfulResult
                {
                    Message = weather.ToString()
                };
            }
            catch
            {
                return new FailedResult(new List<string>
                {
                    "Невозможно получить погоду с внешнего сайта. Попробуйте позже."
                });
            }
        }
    }
}