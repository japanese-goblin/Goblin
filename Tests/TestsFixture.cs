using System;
using System.Net.Http;
using Goblin.WebUI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;

namespace Tests
{
    public class TestsFixture : IDisposable
    {
        public readonly HttpClient Client;

        public TestsFixture()
        {
            //VkApi.SetAccessToken(Settings.AccessToken);
            var server = new TestServer(new WebHostBuilder()
                                        .UseEnvironment("Development")
                                        .UseStartup<Startup>());

            Client = server.CreateClient();
        }

        public void Dispose()
        {
            Client?.Dispose();
        }
    }
}