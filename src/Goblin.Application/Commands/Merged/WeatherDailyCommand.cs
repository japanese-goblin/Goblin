using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Goblin.Application.Abstractions;
using Goblin.Application.Extensions;
using Goblin.Application.Results.Failed;
using Goblin.Domain.Entities;
using Goblin.OpenWeatherMap;
using Newtonsoft.Json;
using VkNet.Model;

namespace Goblin.Application.Commands.Merged
{
    public class WeatherDailyCommand : IKeyboardCommand, ITextCommand
    {
        public bool IsAdminCommand => false;
        public string[] Aliases => new[] { "погодаз" };

        public string Trigger => "weatherDaily";

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
                var text = "Для получения погоды установите город (нужно написать следующее - установить город Москва).";
                return new FailedResult(text);
            }

            return await _api.GetDailyWeatherWithResult(user.WeatherCity, DateTime.Today.AddDays(1));
        }

        private async Task<IResult> ExecutePayload(Message msg, BotUser user)
        {
            if(string.IsNullOrWhiteSpace(user.WeatherCity))
            {
                return new FailedResult("Для получения погоды установите город (нужно написать следующее - установить город Москва).");
            }

            var day = JsonConvert.DeserializeObject<Dictionary<string, string>>(msg.Payload)[Trigger];

            return await _api.GetDailyWeatherWithResult(user.WeatherCity, DateTime.Parse(day));
        }
    }
}