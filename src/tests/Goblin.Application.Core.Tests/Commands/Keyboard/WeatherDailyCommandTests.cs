using FluentAssertions;
using Goblin.Application.Core.Abstractions;
using Goblin.Application.Core.Commands.Keyboard;
using NSubstitute;
using Xunit;

namespace Goblin.Application.Core.Tests.Commands.Keyboard;

public class WeatherDailyCommandTests : TestBase
{
    private static IWeatherService GetWeatherService()
    {
        var mock = Substitute.For<IWeatherService>();
        mock.GetDailyWeather(Arg.Any<string>(), Arg.Any<DateTime>())
            .Returns(CommandExecutionResult.Success("weather"));
        return mock;
    }

    [Fact]
    public async Task ShouldReturnSuccessfulResult()
    {
        var command = new WeatherDailyCommand(GetWeatherService());
        var message = GenerateMessageWithPayload(DefaultUser.Id, DefaultUser.Id, command.Trigger, "11.11.2011");

        var result = await command.Execute(message, DefaultUser);
        result.IsSuccessful.Should().BeTrue();
        result.Message.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task ShouldReturnFailedResult_Because_DictionaryKeyDoesNotExists()
    {
        var command = new WeatherDailyCommand(GetWeatherService());
        var message = GenerateMessageWithPayload(DefaultUser.Id, DefaultUser.Id, "asd", "11.11.2011");

        var result = await command.Execute(message, DefaultUser);
        result.IsSuccessful.Should().BeFalse();
        result.Message.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task ShouldReturnFailedResult_Because_DictionaryValueIsNotCorrectDate()
    {
        var command = new WeatherDailyCommand(GetWeatherService());
        var message = GenerateMessageWithPayload(DefaultUser.Id, DefaultUser.Id, command.Trigger, "20.20.2020");

        var result = await command.Execute(message, DefaultUser);
        result.IsSuccessful.Should().BeFalse();
        result.Message.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task ShouldReturnFailedResult_Because_UserCityIsEmpty()
    {
        DefaultUser.SetCity(string.Empty);
        var command = new WeatherDailyCommand(GetWeatherService());
        var message = GenerateMessageWithPayload(DefaultUser.Id, DefaultUser.Id, command.Trigger, "11.11.2011");

        var result = await command.Execute(message, DefaultUser);
        result.IsSuccessful.Should().BeFalse();
        result.Message.Should().NotBeNullOrEmpty();
    }
}