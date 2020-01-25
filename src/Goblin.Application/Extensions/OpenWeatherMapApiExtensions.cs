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
        private const string SiteIsUnavailable = "Сайт с погодой временно недоступен. Попробуйте позже.";
        private const string UnexpectedError = "Непредвиденная ошибка при получении погоды. Попробуйте позже.";

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
            catch(FlurlHttpException)
            {
                return new FailedResult(SiteIsUnavailable);
            }
            catch(Exception ex)
            {
                Log.ForContext<OpenWeatherMapApi>().Fatal(ex, "Ошибка при получении погоды на текущий момент");
                return new FailedResult(UnexpectedError);
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
            catch(FlurlHttpException)
            {
                return new FailedResult(SiteIsUnavailable);
            }
            catch(Exception ex)
            {
                Log.ForContext<OpenWeatherMapApi>().Fatal(ex, "Ошибка при получении погоды на день");
                return new FailedResult(UnexpectedError);
            }
        }
    }
}