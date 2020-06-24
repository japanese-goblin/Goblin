using System;
using System.Threading.Tasks;

namespace Goblin.Application.Core.Abstractions
{
    public interface IWeatherService
    {
        public Task<IResult> GetCurrentWeather(string city);

        public Task<IResult> GetDailyWeather(string city, DateTime date);
    }
}