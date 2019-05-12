using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Moq.Protected;

namespace Vk.Tests
{
    public class TestBase
    {
        public VkApi GetApi(string data)
        {
            var client = GenerateClient(File.ReadAllText($"data/{data}.json"));
            return new VkApi("super-secret-token", client);
        }
        private HttpClient GenerateClient(string content)
        {
            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                                                  "SendAsync",
                                                  ItExpr.IsAny<HttpRequestMessage>(),
                                                  ItExpr.IsAny<CancellationToken>()
                                                 ) // Setup the PROTECTED method to mock
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(content)
                }) // prepare the expected response of the mocked http call
                .Verifiable();

            var httpClient = new HttpClient(handlerMock.Object)
            {
                BaseAddress = new Uri(VkApi.EndPoint)
            };

            return httpClient;
        }
    }
}
