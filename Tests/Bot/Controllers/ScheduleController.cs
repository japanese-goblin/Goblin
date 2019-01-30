using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace Tests.Bot.Controllers
{
    public class ScheduleController : IClassFixture<TestsFixture>
    {
        private readonly TestsFixture _fixture;

        public ScheduleController(TestsFixture fixt)
        {
            _fixture = fixt;
        }

        [Fact]
        public async Task TestHomePage()
        {
            var response = await _fixture.Client.GetAsync("/Schedule");
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Theory]
        [InlineData(351617)]
        [InlineData(351618)]
        public async Task TestShowPage(int? id = null)
        {
            var response = await _fixture.Client.GetAsync($"Schedule/Show?id={id}");
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Theory]
        [InlineData(15)]
        public async Task TestErrorPage(int? id = null)
        {
            var response = await _fixture.Client.GetAsync($"Schedule/Show?id={id}");
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}
