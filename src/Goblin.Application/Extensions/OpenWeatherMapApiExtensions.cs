using System;
using System.Threading.Tasks;
using Flurl.Http;
using Goblin.Application.Abstractions;
using Goblin.Application.Results.Failed;
using Goblin.Application.Results.Success;
using Goblin.OpenWeatherMap;
using Serilog;

namespace Goblin.Application.Extensions
{
    public static class OpenWeatherMapApiExtensions
    {
        public static async Task<IResult> GetCurrentWeatherWithResult(this OpenWeatherMapApi api, string city)
        {
            try
            {
                var weather = await api.GetCurrentWeather(city);
                return new SuccessfulResult
                {
                    Message = weather.ToString()
                };
            }
            catch(FlurlHttpException ex)
            {
                Log.Error(ex, "OpenWeatherMap API недоступен");
                return new FailedResult("Невозможно получить погоду с сайта.");
            }
            catch(Exception ex)
            {
                return new FailedResult($"Невозможно получить погоду с внешнего сайта ({ex.Message}). Попробуйте позже.");
            }
        }

        public static async Task<IResult> GetDailyWeatherWithResult(this OpenWeatherMapApi api, string city,
                                                                    DateTime date)
        {
            try
            {
                var weather = await api.GetDailyWeatherAt(city, date);
                string formatedDate;
                if(date.Date == DateTime.Today)
                {
                    formatedDate = $"сегодня ({date:dd.MM (dddd)})";
                }
                else if(date.Date == DateTime.Today.AddDays(1))
                {
                    formatedDate = $"завтра ({date:dd.MM (dddd)})";
                }
                else
                {
                    formatedDate = $"({date:dd.MM (dddd)})";
                }
                return new SuccessfulResult
                {
                    Message = $"Погода в городе {city} на {formatedDate}:\n{weather}"
                };
            }
            catch(FlurlHttpException ex)
            {
                Log.Error(ex, "OpenWeatherMap API недоступен");
                return new FailedResult($"Невозможно получить погоду с внешнего сайта (Код ошибки - {ex.Call.HttpStatus}).");
            }
            catch(Exception ex)
            {
                Log.Fatal(ex, "Непредвиденная ошибка при получении погоды");
                return new FailedResult($"Непредвиденная ошибка при получении погоды ({ex.Message}). Попробуйте позже.");
            }
        }
    }
}