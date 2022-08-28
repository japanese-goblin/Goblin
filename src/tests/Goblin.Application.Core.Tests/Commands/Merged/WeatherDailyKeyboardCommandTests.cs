using System.Threading.Tasks;
using FluentAssertions;
using Goblin.Application.Core.Commands.Merged;
using Goblin.Application.Core.Results.Failed;
using Goblin.Application.Core.Results.Success;
using Xunit;

namespace Goblin.Application.Core.Tests.Commands.Merged;

public class WeatherDailyKeyboardCommandTests : TestBase
{
    [Fact]
    public async Task ShouldReturnSuccessfulResult()
    {
        var command = new WeatherDailyKeyboardCommand();
        var message = GenerateMessage(DefaultUser.Id, DefaultUser.Id, command.Aliases[0]);

        var result = await command.Execute(message, DefaultUser);
        result.Should().BeOfType<SuccessfulResult>();
        result.Message.Should().NotBeNullOrEmpty();
        result.Keyboard.Should().NotBeNull();
    }

    [Fact]
    public async Task ShouldReturnFailedResult_Because_UserCityIsEmpty()
    {
        DefaultUser.SetCity(string.Empty);
        var command = new WeatherDailyKeyboardCommand();
        var message = GenerateMessage(DefaultUser.Id, DefaultUser.Id, command.Aliases[0]);

        var result = await command.Execute(message, DefaultUser);
        result.Should().BeOfType<FailedResult>();
        result.Message.Should().NotBeNullOrEmpty();
    }
}