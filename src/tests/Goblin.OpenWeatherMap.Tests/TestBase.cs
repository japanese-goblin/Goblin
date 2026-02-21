using Goblin.OpenWeatherMap.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;

namespace Goblin.OpenWeatherMap.Tests;

public class TestBase
{
    private const string DefaultPath = "TestData";

    protected const string CorrectCity = "Moscow";
    protected const string IncorrectCity = "City17";

    private static string CurrentWeatherPath => Path.Combine(DefaultPath, "current_weather.json");
    private static string DailyWeatherPath => Path.Combine(DefaultPath, "daily_weather.json");

    protected OpenWeatherMap.OpenWeatherMapApi Api { get; init; }

    protected TestBase()
    {
        // var mockHttp = Substitute.For<HttpMessageHandler>();
        //
        // mockHttp.When("*/weather")
        //         .WithQueryString("q", CorrectCity)
        //         .Respond(MediaTypeNames.Application.Json, File.ReadAllText(CurrentWeatherPath));
        // mockHttp.When("*/forecast/daily")
        //         .WithQueryString("q", CorrectCity)
        //         .Respond(MediaTypeNames.Application.Json, File.ReadAllText(DailyWeatherPath));
        //
        // mockHttp.When("*/weather")
        //         .WithQueryString("q", IncorrectCity)
        //         .Respond(HttpStatusCode.NotFound);
        // mockHttp.When("*/forecast/daily")
        //         .WithQueryString("q", IncorrectCity)
        //         .Respond(HttpStatusCode.NotFound);
        //
        // mockHttp.When(HttpMethod.Head, "*/weather*")
        //         .WithQueryString("q", CorrectCity)
        //         .Respond(MediaTypeNames.Application.Json, File.ReadAllText(CurrentWeatherPath));
        // mockHttp.When(HttpMethod.Head, "*/weather*")
        //         .WithQueryString("q", IncorrectCity)
        //         .Respond(HttpStatusCode.NotFound);

        var factory = Substitute.For<IHttpClientFactory>();
        factory.CreateClient(Arg.Any<string>())
               .Returns(Substitute.For<HttpClient>());

        var options = Options.Create(new OpenWeatherMapApiOptions
        {
            AccessToken = "test-token"
        });

        Api = new OpenWeatherMap.OpenWeatherMapApi(factory, options, Substitute.For<ILogger<OpenWeatherMap.OpenWeatherMapApi>>());
    }
}