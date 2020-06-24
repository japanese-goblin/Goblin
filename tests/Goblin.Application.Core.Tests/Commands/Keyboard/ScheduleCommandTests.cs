using System;
using System.Threading.Tasks;
using FluentAssertions;
using Goblin.Application.Core.Abstractions;
using Goblin.Application.Core.Commands.Keyboard;
using Goblin.Application.Core.Results.Failed;
using Goblin.Application.Core.Results.Success;
using Goblin.Domain.Entities;
using Moq;
using Xunit;

namespace Goblin.Application.Core.Tests.Commands.Keyboard
{
    public class ScheduleCommandTests : TestBase
    {
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
            var message = GenerateMessageWithPayload(DefaultUser.Id, DefaultUser.Id, command.Trigger, "22.02.2150");

            var result = await command.Execute<VkBotUser>(message, DefaultUser);
            result.Should().BeOfType<FailedResult>();
            result.Message.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task ShouldReturnSuccessfulResult()
        {
            var command = new ScheduleCommand(GetScheduleService());
            var message = GenerateMessageWithPayload(DefaultUser.Id, DefaultUser.Id, command.Trigger, "22.02.2150");

            var result = await command.Execute<VkBotUser>(message, DefaultUser);
            result.Should().BeOfType<SuccessfulResult>();
            result.Message.Should().NotBeNullOrEmpty();
            result.Keyboard.Should().NotBeNull();
        }
    }
}