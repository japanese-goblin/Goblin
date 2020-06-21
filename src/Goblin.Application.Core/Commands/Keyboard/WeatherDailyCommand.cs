using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Flurl.Http;
using Goblin.Application.Core.Abstractions;
using Goblin.Application.Core.Results.Failed;
using Goblin.Application.Core.Results.Success;
using Goblin.Domain.Abstractions;
using Goblin.OpenWeatherMap;
using Newtonsoft.Json;
using Serilog;

namespace Goblin.Application.Core.Commands.Keyboard
{
    public class WeatherDailyCommand : IKeyboardCommand
    {
        public string Trigger => "weatherDaily";
        private readonly OpenWeatherMapApi _api;

        public WeatherDailyCommand(OpenWeatherMapApi api)
        {
            _api = api;
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

            var weather = await _api.GetDailyWeatherAt(user.WeatherCity, DateTime.Today.AddDays(1));

            return new SuccessfulResult
            {
                Message = weather.ToString(),
                Keyboard = DefaultKeyboards.GetDailyWeatherKeyboard()
            };
        }

        private async Task<IResult> ExecutePayload(IMessage msg, BotUser user)
        {
            if(string.IsNullOrWhiteSpace(user.WeatherCity))
            {
                return new FailedResult("Для получения погоды установите город (нужно написать следующее - установить город Москва).");
            }

            var day = JsonConvert.DeserializeObject<Dictionary<string, string>>(msg.Payload)[Trigger];

            var weather = await _api.GetDailyWeatherAt(user.WeatherCity, DateTime.Parse(day));

            return new SuccessfulResult
            {
                Message = weather.ToString(),
                Keyboard = DefaultKeyboards.GetDailyWeatherKeyboard()
            };
        }

        public async Task<IResult> GetDailyWeather(string city, DateTime date)
        {
            try
            {
                var weather = await _api.GetDailyWeatherAt(city, date);

                string formattedDate;
                if(date.Date == DateTime.Today)
                {
                    formattedDate = $"сегодня ({date:dd.MM, dddd})";
                }
                else if(date.Date == DateTime.Today.AddDays(1))
                {
                    formattedDate = $"завтра ({date:dd.MM, dddd})";
                }
                else
                {
                    formattedDate = $"({date:dd.MM (dddd)})";
                }

                return new SuccessfulResult
                {
                    Message = $"Погода в городе {city} на {formattedDate}:\n{weather}"
                };
            }
            catch(FlurlHttpException ex)
            {
                if(ex.Call.HttpStatus == HttpStatusCode.NotFound)
                {
                    return new FailedResult($"Город \"{city}\" не найден");
                }

                Log.ForContext<OpenWeatherMapApi>().Fatal(ex, "Ошибка при получении погоды на текущий момент");
                return new FailedResult(DefaultErrors.WeatherSiteIsUnavailable);
            }
            catch(Exception ex)
            {
                Log.ForContext<OpenWeatherMapApi>().Fatal(ex, "Ошибка при получении погоды на день");
                return new FailedResult(DefaultErrors.WeatherUnexpectedError);
            }
        }
    }
}