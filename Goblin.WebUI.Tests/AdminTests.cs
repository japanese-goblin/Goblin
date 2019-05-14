using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Goblin.WebUI.Tests
{
    public class AdminTests :  IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly WebApplicationFactory<Startup> _factory;

        public AdminTests(WebApplicationFactory<Startup> factory)
        {
            _factory = factory;
        }

        [Fact(DisplayName = "Check authorization")]
        public async void CheckAuthorize()
        {
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync("/Admin");

            Assert.StartsWith("https://localhost/Identity/Account/Login", 
                              response.RequestMessage.RequestUri.OriginalString);
        }
    }
}