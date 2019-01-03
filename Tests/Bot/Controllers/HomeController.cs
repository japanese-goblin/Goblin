using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace Tests.Bot.Controllers
{
    public class HomeController : IClassFixture<TestsFixture>
    {
        private readonly TestsFixture _fixture;
        public HomeController(TestsFixture fixt)
        {
            _fixture = fixt;
        }

        [Fact]
        public async Task TestHomePage()
        {
            var response = await _fixture.Client.GetAsync("/");
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}
