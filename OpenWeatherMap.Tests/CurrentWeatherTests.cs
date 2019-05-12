using System;
using Xunit;

namespace OpenWeatherMap.Tests
{
    public class CurrentWeatherTests : TestBase
    {
        private const string City = "Город";

        [Fact(DisplayName = "Get current weather")]
        public async void GetWeather()
        {
            var result = await GetWeatherInfo().GetCurrentWeather(City);
            Assert.NotNull(result);
            Assert.Equal("Arkhangelsk", result.CityName);
            Assert.Equal(200, result.ResponseCode);
            Assert.NotEmpty(result.Info);
        }

        [Fact(DisplayName = "Get current weather as string")]
        public async void GetString()
        {
            var result = await GetWeatherInfo().GetCurrentWeatherString(City);
            Assert.NotEmpty(result);
            Assert.Contains("Температура", result);
            Assert.Contains("данный момент", result);
        }

        private WeatherInfo GetWeatherInfo()
        {
            var client = GetCurrentHttpClient();
            return new WeatherInfo("test_token", client);
        }
    }
}