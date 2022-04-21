using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace Goblin.OpenWeatherMap.Tests.OpenWeatherMapApi;

public class IsCityExistsTests : TestBase
{
    [Fact]
    public async Task IsCityExists_CorrectCity_ReturnsTrue()
    {
        var isExists = await Api.IsCityExists(CorrectCity);

        isExists.Should().BeTrue();
    }

    [Fact]
    public async Task IsCityExists_IncorrectCity_ReturnsFalse()
    {
        var isExists = await Api.IsCityExists(IncorrectCity);

        isExists.Should().BeFalse();
    }
}