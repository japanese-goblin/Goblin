using System;
using Xunit;

namespace OpenWeatherMap.Tests
{
    public class DailyWeatherTests : TestBase
    {
        private const string City = "Город";

        [Fact]
        public async void GetWeather()
        {
            var result = await GetWeatherInfo().GetDailyWeather(City, DateTime.Today);
            Assert.Equal("Arkhangelsk", result.City.Name);
            Assert.Equal("200", result.Cod);
            Assert.NotEmpty(result.List);
        }

        [Fact]
        public async void GetString()
        {
            var result = await GetWeatherInfo().GetDailyWeatherString(City, DateTime.Today);
            Assert.Contains("Температура", result);
            Assert.Contains("сегодня", result);
        }

        private WeatherInfo GetWeatherInfo()
        {
            var client = GetDailyHttpClient();
            return new WeatherInfo("test_token", client);
        }
    }
}
