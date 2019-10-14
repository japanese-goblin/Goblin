using System.IO;

namespace Goblin.OpenWeatherMap.Tests
{
    public class TestBase
    {
        protected const string DefaultPath = "TestData";

        protected string CurrentWeatherPath => Path.Combine(DefaultPath, "current_weather.json");
        protected string DailyWeatherPath => Path.Combine(DefaultPath, "daily_weather.json");

        protected const string CorrectCity = "Moscow";
        protected const string IncorrectCity = "City17";
        
        protected OpenWeatherMap.OpenWeatherMapApi Api => new OpenWeatherMap.OpenWeatherMapApi("test_token");
    }
}