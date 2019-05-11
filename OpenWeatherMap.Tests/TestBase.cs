using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Moq.Protected;

namespace OpenWeatherMap.Tests
{
    public class TestBase
    {
        public HttpClient GetDailyHttpClient()
        {
            return GenerateClient(File.ReadAllText("data/daily.json"));
        }

        public HttpClient GetCurrentHttpClient()
        {
            return GenerateClient(File.ReadAllText("data/current.json"));
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
                BaseAddress = new Uri(WeatherInfo.EndPoint)
            };

            return httpClient;
        }
    }
}