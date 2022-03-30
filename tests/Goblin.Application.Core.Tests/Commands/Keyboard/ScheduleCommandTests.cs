using System;
using System.Threading.Tasks;
using FluentAssertions;
using Goblin.Application.Core.Abstractions;
using Goblin.Application.Core.Commands.Keyboard;
using Goblin.Application.Core.Results.Failed;
using Goblin.Application.Core.Results.Success;
using Moq;
using Xunit;

namespace Goblin.Application.Core.Tests.Commands.Keyboard;

public class ScheduleCommandTests : TestBase
{
    private readonly DateTime DateTime = new DateTime(2150, 02, 02);

    private IScheduleService GetScheduleService()
    {
        var mock = new Mock<IScheduleService>();
        mock.Setup(x => x.GetSchedule(It.IsAny<int>(), It.IsAny<DateTime>()))
            .ReturnsAsync(new SuccessfulResult
            {
                Message = "Расписание",
                Keyboard = DefaultKeyboards.GetScheduleKeyboard()
            });

        return mock.Object;
    }

    [Fact]
    public async Task ShouldReturnFailedResult_Because_UserGroupIsZero()
    {
        DefaultUser.SetNarfuGroup(0);
        var command = new ScheduleCommand(GetScheduleService());
        var message = GenerateMessageWithPayload(DefaultUser.Id, DefaultUser.Id, command.Trigger, DateTime.ToString("d"));

        var result = await command.Execute(message, DefaultUser);
        result.Should().BeOfType<FailedResult>();
        result.Message.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task ShouldReturnSuccessfulResult()
    {
        var command = new ScheduleCommand(GetScheduleService());
        var message = GenerateMessageWithPayload(DefaultUser.Id, DefaultUser.Id, command.Trigger, DateTime.ToString("d"));

        var result = await command.Execute(message, DefaultUser);
        result.Should().BeOfType<SuccessfulResult>();
        result.Message.Should().NotBeNullOrEmpty();
        result.Keyboard.Should().NotBeNull();
    }
}