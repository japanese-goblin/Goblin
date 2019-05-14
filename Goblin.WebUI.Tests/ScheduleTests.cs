using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Goblin.WebUI.Tests
{
    public class ScheduleTests : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly WebApplicationFactory<Startup> _factory;

        public ScheduleTests(WebApplicationFactory<Startup> factory)
        {
            _factory = factory;
        }

        [Theory(DisplayName = "Get correct schedule")]
        [InlineData(351818)]
        [InlineData(351717)]
        public async void Show__Correct(int id)
        {
            var client = _factory.CreateClient();
            var response = await client.GetAsync($"/Schedule/Show?Id={id}");
            response.EnsureSuccessStatusCode();
        }

        [Theory(DisplayName = "Get incorrect schedule")]
        [InlineData(1)]
        [InlineData(2)]
        public async void Show__Incorrect(int id)
        {
            var client = _factory.CreateClient();
            var response = await client.GetAsync($"/Schedule/Show?Id={id}");
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
    }
}