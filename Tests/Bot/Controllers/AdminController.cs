using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace Tests.Bot.Controllers
{
    public class AdminController : IClassFixture<TestsFixture>
    {
        private readonly TestsFixture _fixture;

        public AdminController(TestsFixture fixt)
        {
            _fixture = fixt;
        }

        [Fact]
        public async Task TestHomePage()
        {
            var response = await _fixture.Client.GetAsync("/Admin");
            Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
        }

        [Fact]
        public async Task TestLoginPage()
        {
            var response = await _fixture.Client.GetAsync("/Admin/Login");
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task TestLogoutPage()
        {
            var response = await _fixture.Client.GetAsync("/Admin/Logout");
            Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
        }
    }
}