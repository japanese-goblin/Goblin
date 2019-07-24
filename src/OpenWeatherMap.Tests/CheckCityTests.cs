using System.Net;
using Flurl.Http.Testing;
using Xunit;

namespace OpenWeatherMap.Tests
{
    public class CheckCityTests : TestBase
    {
        [Fact]
        public async void CheckCity_CorrectCity_True()
        {
            using(var httpTest = new HttpTest())
            {
                httpTest.RespondWith("");

                var result = await GetService().CheckCity(City);

                Assert.True(result);
            }
        }

        [Fact]
        public async void CheckCity_IncorrectCity_False()
        {
            using(var httpTest = new HttpTest())
            {
                httpTest.RespondWith("", (int)HttpStatusCode.NotFound);

                var result = await GetService().CheckCity("фыв");

                Assert.False(result);
            }
        }
    }
}