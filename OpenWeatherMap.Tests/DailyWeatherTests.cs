using System;
using System.IO;
using System.Net;
using System.Net.Http;
using Flurl.Http.Testing;
using Xunit;

namespace OpenWeatherMap.Tests
{
    public class DailyWeatherTests
    {
        private const string City = "Архангельск";
        private const string DailyDataPath = "data/daily.json";

        [Fact(DisplayName = "Get daily weather")]
        public async void GetWeather()
        {
            using(var httpTest = new HttpTest())
            {
                httpTest.RespondWith(File.ReadAllText(DailyDataPath));

                var wi = new WeatherInfo("SuperToken");
                var result = await wi.GetDailyWeather(City, DateTime.Today);

                httpTest.ShouldHaveCalled($"{WeatherInfo.EndPoint}*")
                        .WithVerb(HttpMethod.Get)
                        .WithHeader("Accept", "application/json")
                        .Times(1);

                Assert.Equal("Arkhangelsk", result.City.Name);
                Assert.Equal("200", result.Cod);
                Assert.NotEmpty(result.List);
            }
        }

        [Fact(DisplayName = "Get daily weather as string")]
        public async void GetString()
        {
            using(var httpTest = new HttpTest())
            {
                httpTest.RespondWith(File.ReadAllText(DailyDataPath));

                var wi = new WeatherInfo("SuperToken");
                var result = await wi.GetDailyWeatherString(City, DateTime.Today);

                httpTest.ShouldHaveCalled($"{WeatherInfo.EndPoint}*")
                        .WithVerb(HttpMethod.Get)
                        .WithHeader("Accept", "application/json")
                        .Times(1);

                Assert.Contains("Температура", result);
                Assert.Contains("сегодня", result);
            }
        }
    }
}
