using System;
using FluentAssertions;
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
            Func<object> func = () => new OpenWeatherMap.OpenWeatherMapApi(token);
            
            func.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Constructor_CorrectToken_CreatesInstance()
        {
            Func<object> func = () => new OpenWeatherMap.OpenWeatherMapApi("test_token");
            
            func.Should().NotThrow();
        }
    }
}