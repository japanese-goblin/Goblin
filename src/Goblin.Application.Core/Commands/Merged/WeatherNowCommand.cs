using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Flurl.Http;
using Goblin.Application.Core.Abstractions;
using Goblin.Application.Core.Results.Failed;
using Goblin.Application.Core.Results.Success;
using Goblin.Domain.Abstractions;
using Goblin.OpenWeatherMap;
using Serilog;

namespace Goblin.Application.Core.Commands.Merged
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

        public async Task<IResult> Execute<T>(IMessage msg, BotUser user) where T : BotUser
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

            if(string.IsNullOrWhiteSpace(user.WeatherCity) && string.IsNullOrWhiteSpace(city))
            {
                return new FailedResult(DefaultErrors.CityNotSet);
            }

            if(!string.IsNullOrWhiteSpace(city))
            {
                return await GetWeather(city);
            }

            return await GetWeather(user.WeatherCity);
        }

        private async Task<IResult> ExecutePayload(BotUser user)
        {
            if(string.IsNullOrWhiteSpace(user.WeatherCity))
            {
                return new FailedResult(DefaultErrors.CityNotSet);
            }

            return await GetWeather(user.WeatherCity);
        }

        public async Task<IResult> GetWeather(string city)
        {
            try
            {
                var weather = await _api.GetCurrentWeather(city);
                return new SuccessfulResult
                {
                    Message = weather.ToString()
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
                Log.ForContext<OpenWeatherMapApi>().Fatal(ex, "Ошибка при получении погоды на текущий момент");
                return new FailedResult(DefaultErrors.WeatherUnexpectedError);
            }
        }
    }
}