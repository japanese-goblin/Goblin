using System.IO;

namespace Goblin.OpenWeatherMap.Tests;

public class TestBase
{
    private const string DefaultPath = "TestData";

    protected const string CorrectCity = "Moscow";
    protected const string IncorrectCity = "City17";

    protected string CurrentWeatherPath => Path.Combine(DefaultPath, "current_weather.json");
    protected string DailyWeatherPath => Path.Combine(DefaultPath, "daily_weather.json");

    protected OpenWeatherMap.OpenWeatherMapApi Api => new OpenWeatherMap.OpenWeatherMapApi("test_token");
}