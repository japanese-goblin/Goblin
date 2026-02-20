using FluentAssertions;
using Goblin.Application.Core.Abstractions;
using Goblin.Application.Core.Commands.Keyboard;
using Goblin.Application.Core.Results.Failed;
using Goblin.Application.Core.Results.Success;
using NSubstitute;
using Xunit;

namespace Goblin.Application.Core.Tests.Commands.Keyboard;

public class ScheduleCommandTests : TestBase
{
    private readonly DateTime _dateTime = new DateTime(2150, 02, 02);

    private static IScheduleService GetScheduleService()
    {
        var mock = Substitute.For<IScheduleService>();
        mock.GetSchedule(Arg.Any<int>(), Arg.Any<DateTime>())
            .Returns(new SuccessfulResult
            {
                Message = "Расписание",
                Keyboard = DefaultKeyboards.GetScheduleKeyboard()
            });

        return mock;
    }

    [Fact]
    public async Task ShouldReturnFailedResult_Because_UserGroupIsZero()
    {
        DefaultUser.SetNarfuGroup(0);
        var command = new ScheduleCommand(GetScheduleService());
        var message = GenerateMessageWithPayload(DefaultUser.Id, DefaultUser.Id, command.Trigger, _dateTime.ToString("d"));

        var result = await command.Execute(message, DefaultUser);
        result.Should().BeOfType<FailedResult>();
        result.Message.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task ShouldReturnSuccessfulResult()
    {
        var command = new ScheduleCommand(GetScheduleService());
        var message = GenerateMessageWithPayload(DefaultUser.Id, DefaultUser.Id, command.Trigger, _dateTime.ToString("d"));

        var result = await command.Execute(message, DefaultUser);
        result.Should().BeOfType<SuccessfulResult>();
        result.Message.Should().NotBeNullOrEmpty();
        result.Keyboard.Should().NotBeNull();
    }
}