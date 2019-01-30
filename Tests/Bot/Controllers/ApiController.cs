using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace Tests.Bot.Controllers
{
    public class ApiController : IClassFixture<TestsFixture>
    {
        private readonly TestsFixture _fixture;

        public ApiController(TestsFixture fixt)
        {
            _fixture = fixt;
        }

        [Fact]
        public async Task TestHomePage()
        {
            var response = await _fixture.Client.GetAsync("/Api");
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}
