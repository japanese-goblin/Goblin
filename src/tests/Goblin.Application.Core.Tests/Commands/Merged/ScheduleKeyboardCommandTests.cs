using FluentAssertions;
using Goblin.Application.Core.Commands.Merged;
using Xunit;

namespace Goblin.Application.Core.Tests.Commands.Merged;

public class ScheduleKeyboardCommandTests : TestBase
{
    [Fact]
    public async Task ShouldReturnSuccessfulResult()
    {
        var command = new ScheduleKeyboardCommand();
        var message = GenerateMessage(DefaultUser.Id, DefaultUser.Id, command.Aliases[0]);

        var result = await command.Execute(message, DefaultUser);
        result.IsSuccessful.Should().BeTrue();
        result.Message.Should().NotBeNullOrEmpty();
        result.Keyboard.Should().NotBeNull();
    }

    [Fact]
    public async Task ShouldReturnFailedResult_Because_UserGroupIsZero()
    {
        DefaultUser.SetNarfuGroup(0);
        var command = new ScheduleKeyboardCommand();
        var message = GenerateMessage(DefaultUser.Id, DefaultUser.Id, command.Aliases[0]);

        var result = await command.Execute(message, DefaultUser);
        result.IsSuccessful.Should().BeFalse();
        result.Message.Should().NotBeNullOrEmpty();
    }
}