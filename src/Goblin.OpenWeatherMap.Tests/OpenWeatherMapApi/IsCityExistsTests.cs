using System.Threading.Tasks;
using Flurl.Http.Testing;
using Xunit;

namespace Goblin.OpenWeatherMap.Tests.OpenWeatherMapApi
{
    public class IsCityExistsTests : TestBase
    {
        [Fact]
        public async Task IsCityExists_CorrectCity_ReturnsTrue()
        {
            using var http = new HttpTest();
            http.RespondWith(string.Empty);
            var isExists = await Api.IsCityExists(CorrectCity);

            Assert.True(isExists);
        }

        [Fact]
        public async Task IsCityExists_IncorrectCity_ReturnsTrue()
        {
            using var http = new HttpTest();
            http.RespondWith(string.Empty, 404);
            var isExists = await Api.IsCityExists(IncorrectCity);

            Assert.False(isExists);
        }
    }
}