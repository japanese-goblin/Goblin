using System;
using System.Threading.Tasks;
using Flurl.Http;
using Goblin.Application.Abstractions;
using Goblin.Application.Results.Failed;
using Goblin.Application.Results.Success;
using Goblin.OpenWeatherMap;

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
                return new FailedResult($"Невозможно получить погоду с сайта (код ошибки - {ex.Call.HttpStatus}/{ex.GetType()}.");
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
                
                string formattedDate;
                if(date.Date == DateTime.Today)
                {
                    formattedDate = $"сегодня ({date:dd.MM (dddd)})";
                }
                else if(date.Date == DateTime.Today.AddDays(1))
                {
                    formattedDate = $"завтра ({date:dd.MM (dddd)})";
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
                return new FailedResult($"Невозможно получить погоду с внешнего сайта (Код ошибки - {ex.Call.HttpStatus}/{ex.GetType()}).");
            }
            catch(Exception ex)
            {
                return new FailedResult($"Непредвиденная ошибка при получении погоды ({ex.Message}). Попробуйте позже.");
            }
        }
    }
}