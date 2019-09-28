using System.IO;
using System.Threading.Tasks;
using Flurl.Http;
using Flurl.Http.Testing;
using Xunit;

namespace Goblin.OpenWeatherMap.Tests.OpenWeatherMapApi
{
    public class GetCurrentWeatherTests : TestBase
    {
        [Fact]
        public async Task GetCurrentWeather_CorrectCity_ReturnsModel()
        {
            var api = GetApi();
            using(var http = new HttpTest())
            {
                http.RespondWith(File.ReadAllText(CurrentWeatherPath));
                var weather = await api.GetCurrentWeather(CorrectCity);   
                
                Assert.NotNull(weather);
                Assert.NotNull(weather.Coord);
                Assert.NotEmpty(weather.Info);
                Assert.NotNull(weather.Weather);
                Assert.True(weather.Visibility >= 0);
                Assert.NotNull(weather.Wind);
                Assert.NotNull(weather.Clouds);
                Assert.NotNull(weather.OtherInfo);
                Assert.True(weather.CityName == "Moscow");
                Assert.True(weather.ResponseCode == 200);
                Assert.NotEmpty(weather.ToString());
            }
        }

        [Fact]
        public async Task GetCurrentWeather_IncorrectCity_ThrowsException()
        {
            var api = GetApi();
            using(var http = new HttpTest())
            {
                http.RespondWith(string.Empty, 404);
                await Assert.ThrowsAsync<FlurlHttpException>(async () => await api.GetCurrentWeather(IncorrectCity));
            }
        }
    }
}