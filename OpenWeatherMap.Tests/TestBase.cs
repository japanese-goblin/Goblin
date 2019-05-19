namespace OpenWeatherMap.Tests
{
    public class TestBase
    {
        public const string Token = "Super-token";
        public const string City = "Архангельск";

        public WeatherService GetService()
        {
            return new WeatherService(Token);
        }
    }
}