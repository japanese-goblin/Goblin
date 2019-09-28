using System;
using System.IO;
using System.Threading.Tasks;
using Flurl.Http;
using Flurl.Http.Testing;
using Xunit;

namespace Goblin.OpenWeatherMap.Tests.OpenWeatherMapApi
{
    public class GetDailyWeatherAtTests : TestBase
    {
        [Fact]
        public async Task GetDailyWeatherAt_CorrectCityAndDate_ReturnsModel()
        {
            var api = GetApi();
            using(var http = new HttpTest())
            {
                http.RespondWith(File.ReadAllText(DailyWeatherPath));
                var weather = await api.GetDailyWeatherAt(CorrectCity, DateTime.Today);
                
                Assert.NotNull(weather);
                Assert.True(weather.UnixTime >= 0);
                Assert.NotNull(weather.Temp);
                Assert.True(weather.Pressure >= 0);
                Assert.True(weather.Humidity >= 0);
                Assert.NotEmpty(weather.Weather);
                Assert.True(weather.WindSpeed >= 0);
                Assert.True(weather.WindDeg >= 0 && weather.WindDeg <= 360);
                Assert.True(weather.Cloudiness >= 0 && weather.Cloudiness <= 100);
                Assert.NotEmpty(weather.ToString());
            }
        }

        [Fact]
        public async Task GetDailyWeatherAt_IncorrectCity_ThrowsException()
        {
            var api = GetApi();
            using(var http = new HttpTest())
            {
                http.RespondWith(string.Empty, 404);
                await Assert.ThrowsAsync<FlurlHttpException>(async () =>
                                                                     await api.GetDailyWeatherAt(IncorrectCity, DateTime.Today));
            }
        }
        
        [Fact]
        public async Task GetDailyWeatherAt_IncorrectDate_ThrowsException()
        {
            var api = GetApi();
            using(var http = new HttpTest())
            {
                http.RespondWith(string.Empty);
                await Assert.ThrowsAsync<ArgumentException>(async () =>
                                                                     await api.GetDailyWeatherAt(CorrectCity, DateTime.Today.AddDays(17)));
            }
        }
    }
}