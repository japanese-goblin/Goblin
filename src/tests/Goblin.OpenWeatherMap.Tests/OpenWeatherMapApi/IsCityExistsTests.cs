using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace Goblin.OpenWeatherMap.Tests.OpenWeatherMapApi;

public class IsCityExistsTests : TestBase
{
    [Fact(Skip = "Доработать с NSubstitute")]
    public async Task IsCityExists_CorrectCity_ReturnsTrue()
    {
        var isExists = await Api.IsCityExists(CorrectCity);

        isExists.Should().BeTrue();
    }

    [Fact(Skip = "Доработать с NSubstitute")]
    public async Task IsCityExists_IncorrectCity_ReturnsFalse()
    {
        var isExists = await Api.IsCityExists(IncorrectCity);

        isExists.Should().BeFalse();
    }
}