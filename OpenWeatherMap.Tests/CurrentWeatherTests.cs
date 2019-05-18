using System;
using System.IO;
using System.Net.Http;
using Flurl.Http.Testing;
using Xunit;

namespace OpenWeatherMap.Tests
{
    public class CurrentWeatherTests
    {
        private const string City = "Архангельск";
        private const string CurrentDataPath = "data/current.json";

        [Fact(DisplayName = "Get current weather")]
        public async void GetWeather()
        {
            using(var httpTest = new HttpTest())
            {
                httpTest.RespondWith(File.ReadAllText(CurrentDataPath));

                var wi = new WeatherService("Token");
                var result = await wi.GetCurrentWeather(City);

                httpTest.ShouldHaveCalled($"{WeatherService.EndPoint}*")
                        .WithVerb(HttpMethod.Get)
                        .WithHeader("Accept", "application/json")
                        .Times(1);

                Assert.NotNull(result);
                Assert.Equal("Arkhangelsk", result.CityName);
                Assert.Equal(200, result.ResponseCode);
                Assert.NotEmpty(result.Info);
            }
        }

        [Fact(DisplayName = "Get current weather as string")]
        public async void GetString()
        {
            using(var httpTest = new HttpTest())
            {
                httpTest.RespondWith(File.ReadAllText(CurrentDataPath));

                var wi = new WeatherService("Token");
                var result = await wi.GetCurrentWeatherString(City);

                httpTest.ShouldHaveCalled($"{WeatherService.EndPoint}*")
                        .WithVerb(HttpMethod.Get)
                        .WithHeader("Accept", "application/json")
                        .Times(1);

                Assert.NotNull(result);
                Assert.NotEmpty(result);
                Assert.Contains("Температура", result);
                Assert.Contains("данный момент", result);
            }
        }
    }
}