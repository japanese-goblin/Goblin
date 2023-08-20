using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Mime;
using Microsoft.Extensions.Logging;
using Moq;
using RichardSzalay.MockHttp;

namespace Goblin.OpenWeatherMap.Tests;

public class TestBase
{
    private const string DefaultPath = "TestData";

    protected const string CorrectCity = "Moscow";
    protected const string IncorrectCity = "City17";

    private static string CurrentWeatherPath => Path.Combine(DefaultPath, "current_weather.json");
    private static string DailyWeatherPath => Path.Combine(DefaultPath, "daily_weather.json");

    protected OpenWeatherMap.OpenWeatherMapApi Api { get; init; }

    public TestBase()
    {
        var mockHttp = new MockHttpMessageHandler();
        
        mockHttp.When("*/weather")
                .WithQueryString("q", CorrectCity)
                .Respond(MediaTypeNames.Application.Json, File.ReadAllText(CurrentWeatherPath));
        mockHttp.When("*/forecast/daily")
                .WithQueryString("q", CorrectCity)
                .Respond(MediaTypeNames.Application.Json, File.ReadAllText(DailyWeatherPath));

        mockHttp.When("*/weather")
                .WithQueryString("q", IncorrectCity)
                .Respond(HttpStatusCode.NotFound);
        mockHttp.When("*/forecast/daily")
                .WithQueryString("q", IncorrectCity)
                .Respond(HttpStatusCode.NotFound);
        
        mockHttp.When(HttpMethod.Head, "*/weather*")
                .WithQueryString("q", CorrectCity)
                .Respond(MediaTypeNames.Application.Json, File.ReadAllText(CurrentWeatherPath));
        mockHttp.When(HttpMethod.Head, "*/weather*")
                .WithQueryString("q", IncorrectCity)
                .Respond(HttpStatusCode.NotFound);

        var factory = new Mock<IHttpClientFactory>();
        factory.Setup(x => x.CreateClient(It.IsAny<string>()))
               .Returns(mockHttp.ToHttpClient());

        Api = new OpenWeatherMap.OpenWeatherMapApi("test-token", factory.Object, Mock.Of<ILogger<OpenWeatherMap.OpenWeatherMapApi>>());
    }
}