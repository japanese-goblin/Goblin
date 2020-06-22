using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Goblin.Application.Core.Abstractions;
using Goblin.Application.Core.Results.Failed;
using Goblin.Application.Core.Services;
using Goblin.Domain.Abstractions;
using Newtonsoft.Json;

namespace Goblin.Application.Core.Commands.Keyboard
{
    public class WeatherDailyCommand : IKeyboardCommand
    {
        public string Trigger => "weatherDaily";

        private readonly WeatherService _weatherService;

        public WeatherDailyCommand(WeatherService weatherService)
        {
            _weatherService = weatherService;
        }

        public async Task<IResult> Execute<T>(IMessage msg, BotUser user) where T : BotUser
        {
            if(!string.IsNullOrWhiteSpace(msg.Payload))
            {
                return await ExecutePayload(msg, user);
            }

            return await ExecuteText(user);
        }

        private async Task<IResult> ExecuteText(BotUser user)
        {
            if(string.IsNullOrWhiteSpace(user.WeatherCity))
            {
                const string text = "Для получения погоды установите город (нужно написать следующее - установить город Москва).";
                return new FailedResult(text);
            }

            var weather = await _weatherService.GetDailyWeather(user.WeatherCity, DateTime.Today.AddDays(1));

            return weather;
        }

        private async Task<IResult> ExecutePayload(IMessage msg, BotUser user)
        {
            if(string.IsNullOrWhiteSpace(user.WeatherCity))
            {
                return new FailedResult("Для получения погоды установите город (нужно написать следующее - установить город Москва).");
            }

            var day = JsonConvert.DeserializeObject<Dictionary<string, string>>(msg.Payload)[Trigger];

            var weather = await _weatherService.GetDailyWeather(user.WeatherCity, DateTime.Parse(day));

            return weather;
        }
    }
}