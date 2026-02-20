using FluentAssertions;
using Goblin.Application.Core.Abstractions;
using Goblin.Application.Core.Commands.Merged;
using NSubstitute;
using Xunit;

namespace Goblin.Application.Core.Tests.Commands.Merged;

public class WeatherNowCommandTests : TestBase
{
    private static IWeatherService GetWeatherService()
    {
        var mock = Substitute.For<IWeatherService>();
        mock.GetCurrentWeather(Arg.Any<string>())
            .Returns(CommandExecutionResult.Success("weather"));
        return mock;
    }

    [Fact]
    public async Task ShouldReturnSuccessfulResultWithText_Because_UserHasSetCity()
    {
        var command = new WeatherNowCommand(GetWeatherService());
        var message = GenerateMessage(DefaultUser.Id, DefaultUser.Id, command.Aliases[0]);

        var result = await command.Execute(message, DefaultUser);
        result.IsSuccessful.Should().BeTrue();
        result.Message.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task ShouldReturnFailedResult_Because_UserCityAndParameterAreEmpty()
    {
        DefaultUser.SetCity(string.Empty);
        var command = new WeatherNowCommand(GetWeatherService());
        var message = GenerateMessage(DefaultUser.Id, DefaultUser.Id, command.Aliases[0]);

        var result = await command.Execute(message, DefaultUser);
        result.IsSuccessful.Should().BeFalse();
        result.Message.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task ShouldReturnSuccessfulResult_Because_ParameterIsNotEmpty()
    {
        var command = new WeatherNowCommand(GetWeatherService());
        var text = $"{command.Aliases[0]} Москва";
        var message = GenerateMessage(DefaultUser.Id, DefaultUser.Id, text);

        var result = await command.Execute(message, DefaultUser);
        result.IsSuccessful.Should().BeTrue();
        result.Message.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task ShouldReturnSuccessfulResultWithPayload_Because_UserHasSetCity()
    {
        var command = new WeatherNowCommand(GetWeatherService());
        var message = GenerateMessageWithPayload(DefaultUser.Id, DefaultUser.Id, command.Trigger, string.Empty);

        var result = await command.Execute(message, DefaultUser);
        result.IsSuccessful.Should().BeTrue();
        result.Message.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task ShouldReturnFailedResultWithPayload_Because_UserDoesNotSetCity()
    {
        DefaultUser.SetCity(string.Empty);
        var command = new WeatherNowCommand(GetWeatherService());
        var message = GenerateMessageWithPayload(DefaultUser.Id, DefaultUser.Id, command.Trigger, string.Empty);

        var result = await command.Execute(message, DefaultUser);
        result.IsSuccessful.Should().BeFalse();
        result.Message.Should().NotBeNullOrEmpty();
    }
}