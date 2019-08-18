using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Goblin.Application.Abstractions;
using Goblin.Application.Results;
using Goblin.Domain.Entities;
using Goblin.OpenWeatherMap;
using Newtonsoft.Json;
using VkNet.Model;

namespace Goblin.Application.Commands.Merged
{
    public class WeatherDailyCommand : IKeyboardCommand, ITextCommand
    {
        public string Trigger => "weather";

        public bool IsAdminCommand => false;
        public string[] Aliases => new[] { "погодаз" };

        private readonly OpenWeatherMapApi _api;

        public WeatherDailyCommand(OpenWeatherMapApi api)
        {
            _api = api;
        }

        public async Task<IResult> Execute(Message msg, BotUser user)
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
                return new FailedResult("Для получения погоды сначала необходимо установить город.");
            }
            
            return await GetWeather(user, DateTime.Today.AddDays(1));
        }

        private async Task<IResult> ExecutePayload(Message msg, BotUser user)
        {
            if(string.IsNullOrWhiteSpace(user.WeatherCity))
            {
                return new FailedResult("Для получения погоды сначала необходимо установить город.");
            }

            var day = JsonConvert.DeserializeObject<Dictionary<string, string>>(msg.Payload)["weather"];

            return await GetWeather(user, DateTime.Parse(day));
        }

        private async Task<IResult> GetWeather(BotUser user, DateTime day)
        {
            try
            {
                var weather = await _api.GetDailyWeatherAt(user.WeatherCity, day);
                return new SuccessfulResult
                {
                    Message = $"Погода в городе {user.WeatherCity} на {day:dddd (dd.MM)}: {weather}"
                };
            }
            catch
            {
                return new FailedResult("Невозможно получить погоду с внешнего сайта. Попробуйте позже.");
            }
        }
    }
}