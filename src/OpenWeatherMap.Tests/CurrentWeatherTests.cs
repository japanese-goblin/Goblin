using System.IO;
using System.Net.Http;
using Flurl.Http.Testing;
using Xunit;

namespace OpenWeatherMap.Tests
{
    public class CurrentWeatherTests : TestBase
    {
        private const string CurrentDataPath = "data/current.json";

        [Fact]
        public async void GetCurrentWeather_CorrectCity()
        {
            using(var httpTest = new HttpTest())
            {
                httpTest.RespondWith(File.ReadAllText(CurrentDataPath));

                var result = await GetService().GetCurrentWeather(City);

                httpTest.ShouldHaveCalled($"{WeatherService.EndPoint}*")
                        .WithVerb(HttpMethod.Get)
                        .WithQueryParamValue("q", City)
                        .Times(1);

                Assert.NotNull(result);
                Assert.Equal("Arkhangelsk", result.CityName);
                Assert.Equal(200, result.ResponseCode);
                Assert.NotEmpty(result.Info);
            }
        }

        [Fact]
        public async void GetCurrentWeatherString_CorrectCity()
        {
            using(var httpTest = new HttpTest())
            {
                httpTest.RespondWith(File.ReadAllText(CurrentDataPath));

                var result = await GetService().GetCurrentWeatherString(City);

                Assert.NotNull(result);
                Assert.NotEmpty(result);
                Assert.Contains("Температура", result);
                Assert.Contains("данный момент", result);
            }
        }
    }
}