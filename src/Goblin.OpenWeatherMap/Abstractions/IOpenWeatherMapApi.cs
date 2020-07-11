using System;
using System.Threading.Tasks;
using Goblin.OpenWeatherMap.Models.Current;
using Goblin.OpenWeatherMap.Models.Daily;
using Goblin.OpenWeatherMap.Models.Responses;

namespace Goblin.OpenWeatherMap.Abstractions
{
    public interface IOpenWeatherMapApi
    {
        public Task<CurrentWeatherResponse> GetCurrentWeather(string city);

        public Task<DailyWeatherListItem> GetDailyWeatherAt(string city, DateTime date);

        public Task<bool> IsCityExists(string city);
    }
}