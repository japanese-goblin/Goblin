using System;
using System.Net.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Goblin;

namespace Tests
{
    public class TestsFixture : IDisposable
    {
        public readonly HttpClient Client;

        public TestsFixture()
        {
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
