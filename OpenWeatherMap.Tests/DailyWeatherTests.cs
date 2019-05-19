using System;
using System.IO;
using System.Net.Http;
using Flurl.Http.Testing;
using Xunit;

namespace OpenWeatherMap.Tests
{
    public class DailyWeatherTests : TestBase
    {
        private const string DailyDataPath = "data/daily.json";

        [Fact]
        public async void GetDailyWeather_CorrectCity()
        {
            using(var httpTest = new HttpTest())
            {
                httpTest.RespondWith(File.ReadAllText(DailyDataPath));

                var result = await GetService().GetDailyWeather(City, DateTime.Today);

                httpTest.ShouldHaveCalled($"{WeatherService.EndPoint}*")
                        .WithVerb(HttpMethod.Get)
                        .WithQueryParamValue("q", City)
                        .WithQueryParam("cnt")
                        .Times(1);

                Assert.Equal("Arkhangelsk", result.City.Name);
                Assert.Equal("200", result.Cod);
                Assert.NotEmpty(result.List);
            }
        }

        [Fact]
        public async void GetDailyWeatherString_CorrectCity()
        {
            using(var httpTest = new HttpTest())
            {
                httpTest.RespondWith(File.ReadAllText(DailyDataPath));

                var result = await GetService().GetDailyWeatherString(City, DateTime.Today);

                httpTest.ShouldHaveCalled($"{WeatherService.EndPoint}*")
                        .WithVerb(HttpMethod.Get)
                        .WithHeader("Accept", "application/json")
                        .Times(1);

                Assert.Contains("Температура", result);
                Assert.Contains("сегодня", result);
            }
        }
    }
}