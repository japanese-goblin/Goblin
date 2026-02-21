using FluentAssertions;
using Goblin.Application.Core.Commands.Keyboard;
using Xunit;

namespace Goblin.Application.Core.Tests.Commands.Keyboard;

public class MailingCommandTests : TestBase
{
    [Fact]
    public async Task ShouldReturnFailedResult_Because_CityIsEmpty()
    {
        DefaultUser.SetCity(string.Empty);
        var command = new MailingCommand(GetDbContext());
        var message = GenerateMessageWithPayload(DefaultUser.Id, DefaultUser.Id, command.Trigger, "weather");

        var result = await command.Execute(message, DefaultUser);
        result.IsSuccessful.Should().BeFalse();
        result.Message.Should().Be(DefaultErrors.CityNotSet);
    }

    [Fact]
    public async Task ShouldReturnFailedResult_Because_UnknownValue()
    {
        var command = new MailingCommand(GetDbContext());
        var message = GenerateMessageWithPayload(DefaultUser.Id, DefaultUser.Id, command.Trigger, "asd");

        var result = await command.Execute(message, DefaultUser);
        result.IsSuccessful.Should().BeFalse();
        result.Message.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task ShouldReturnFailedResult_Because_UserGroupIsZero()
    {
        DefaultUser.SetNarfuGroup(0);
        var command = new MailingCommand(GetDbContext());
        var message = GenerateMessageWithPayload(DefaultUser.Id, DefaultUser.Id, command.Trigger, "schedule");

        var result = await command.Execute(message, DefaultUser);
        result.IsSuccessful.Should().BeFalse();
        result.Message.Should().Be(DefaultErrors.GroupNotSet);
    }

    [Fact]
    public async Task ShouldReturnSuccessfulResult_Because_ValueIsSchedule()
    {
        var command = new MailingCommand(GetDbContext());
        var message = GenerateMessageWithPayload(DefaultUser.Id, DefaultUser.Id, command.Trigger, "schedule");

        var result = await command.Execute(message, DefaultUser);
        result.IsSuccessful.Should().BeTrue();
        result.Message.Should().NotBeNullOrEmpty();
        result.Keyboard.Should().NotBeNull();
        result.Keyboard.Buttons.Should().HaveCount(3);
    }

    [Fact]
    public async Task ShouldReturnSuccessfulResult_Because_ValueIsWeather()
    {
        var command = new MailingCommand(GetDbContext());
        var message = GenerateMessageWithPayload(DefaultUser.Id, DefaultUser.Id, command.Trigger, "weather");

        var result = await command.Execute(message, DefaultUser);
        result.IsSuccessful.Should().BeTrue();
        result.Message.Should().NotBeNullOrEmpty();
        result.Keyboard.Should().NotBeNull();
        result.Keyboard.Buttons.Should().HaveCount(3);
    }
}