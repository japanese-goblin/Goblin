using System.Threading.Tasks;
using Xunit;

namespace Tests.Weather
{
    public class WeatherInfo
    {
        private const string ValidCity = "Архангельск";

        [Fact]
        public async Task CheckCity_Valid_True()
        {
            var result = await Goblin.Weather.WeatherInfo.CheckCity(ValidCity);
            Assert.True(result);
        }

        [Fact]
        public async Task CheckCity_NotValid_False()
        {
            var result = await Goblin.Weather.WeatherInfo.CheckCity("мсилшьт");
            Assert.False(result);
        }

        [Fact]
        public async Task GetWeather_Valid_String()
        {
            var result = await Goblin.Weather.WeatherInfo.GetWeather(ValidCity);
            Assert.False(string.IsNullOrEmpty(result));
        }
    }
}
