using System;
using System.Net;
using System.Threading.Tasks;
using Flurl.Http;
using Goblin.Application.Core.Abstractions;
using Goblin.Application.Core.Results.Failed;
using Goblin.Application.Core.Results.Success;
using Goblin.OpenWeatherMap;
using Serilog;

namespace Goblin.Application.Core.Services
{
    public class WeatherService
    {
        private readonly OpenWeatherMapApi _weatherMapApi;

        public WeatherService(OpenWeatherMapApi weatherMapApi)
        {
            _weatherMapApi = weatherMapApi;
        }

        public async Task<IResult> GetCurrentWeather(string city)
        {
            try
            {
                var weather = await _weatherMapApi.GetCurrentWeather(city);
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

        public async Task<IResult> GetDailyWeather(string city, DateTime date)
        {
            try
            {
                var weather = await _weatherMapApi.GetDailyWeatherAt(city, date);

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