using Goblin;
using Goblin.Bot;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using System;
using System.Net.Http;
using Vk;

namespace Tests
{
    public class TestsFixture : IDisposable
    {
        public readonly HttpClient Client;

        public TestsFixture()
        {
            VkApi.SetAccessToken(Settings.AccessToken);
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
