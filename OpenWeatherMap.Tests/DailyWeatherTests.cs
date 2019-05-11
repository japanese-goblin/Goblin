using System;
using Xunit;

namespace OpenWeatherMap.Tests
{
    public class DailyWeatherTests : TestBase
    {
        [Fact]
        public async void Test1()
        {
            var client = GetDailyHttpClient();
            var wi = new WeatherInfo("test_token", client);
            var x = await wi.GetDailyWeather("Город", DateTime.Today);
        }
    }
}
