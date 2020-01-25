using System;
using Xunit;

namespace Goblin.OpenWeatherMap.Tests.OpenWeatherMapApi
{
    public class ConstructorTests
    {
        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("  ")]
        public void Constructor_IncorrectToken_ThrowsException(string token)
        {
            Assert.Throws<ArgumentException>(() => new OpenWeatherMap.OpenWeatherMapApi(token));
        }

        [Fact]
        public void Constructor_CorrectToken_CreatesInstance()
        {
            _ = new OpenWeatherMap.OpenWeatherMapApi("test_token");
        }
    }
}