using System.Threading.Tasks;
using FluentAssertions;
using Goblin.Application.Core.Commands.Merged;
using Goblin.Application.Core.Results.Failed;
using Goblin.Application.Core.Results.Success;
using Goblin.Domain.Entities;
using Xunit;

namespace Goblin.Application.Core.Tests.Commands.Merged
{
    public class ScheduleKeyboardCommandTests : TestBase
    {
        [Fact]
        public async Task ShouldReturnSuccessfulResult()
        {
            var command = new ScheduleKeyboardCommand();
            var message = GenerateMessage(DefaultUser.Id, DefaultUser.Id, command.Aliases[0]);

            var result = await command.Execute<VkBotUser>(message, DefaultUser);
            result.Should().BeOfType<SuccessfulResult>();
            result.Message.Should().NotBeNullOrEmpty();
            result.Keyboard.Should().NotBeNull();
        }

        [Fact]
        public async Task ShouldReturnFailedResult_Because_UserGroupIsZero()
        {
            DefaultUser.SetNarfuGroup(0);
            var command = new ScheduleKeyboardCommand();
            var message = GenerateMessage(DefaultUser.Id, DefaultUser.Id, command.Aliases[0]);

            var result = await command.Execute<VkBotUser>(message, DefaultUser);
            result.Should().BeOfType<FailedResult>();
            result.Message.Should().NotBeNullOrEmpty();
        }
    }
}