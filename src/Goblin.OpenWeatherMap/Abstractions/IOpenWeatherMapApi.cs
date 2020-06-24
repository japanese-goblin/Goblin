using System;
using System.Threading.Tasks;
using Goblin.OpenWeatherMap.Models.Current;
using Goblin.OpenWeatherMap.Models.Daily;

namespace Goblin.OpenWeatherMap.Abstractions
{
    public interface IOpenWeatherMapApi
    {
        public Task<CurrentWeather> GetCurrentWeather(string city);

        public Task<DailyWeatherListItem> GetDailyWeatherAt(string city, DateTime date);

        public Task<bool> IsCityExists(string city);
    }
}