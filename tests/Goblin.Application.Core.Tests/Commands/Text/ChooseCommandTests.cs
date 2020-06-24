using System.Threading.Tasks;
using FluentAssertions;
using Goblin.Application.Core.Commands.Text;
using Goblin.Application.Core.Results.Failed;
using Goblin.Application.Core.Results.Success;
using Goblin.Domain.Entities;
using Xunit;

namespace Goblin.Application.Core.Tests.Commands.Text
{
    public class ChooseCommandTests : TestBase
    {
        [Theory]
        [InlineData("1 или 2")]
        [InlineData("1,2")]
        [InlineData("1, 2")]
        [InlineData("1,2, 3 или 4")]
        public async Task ShouldReturnSuccessfulResult(string parameters)
        {
            var command = new ChooseCommand();
            var text = $"{command.Aliases[0]} {parameters}";
            var message = GenerateMessage(DefaultUser.Id, DefaultUser.Id, text, string.Empty);

            var result = await command.Execute<VkBotUser>(message, DefaultUser);
            result.Should().BeOfType<SuccessfulResult>();
            result.Message.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task ShouldReturnFailedResult_Because_NotEnoughParameters()
        {
            var command = new ChooseCommand();
            var text = $"{command.Aliases[0]}";
            var message = GenerateMessage(DefaultUser.Id, DefaultUser.Id, text, string.Empty);

            var result = await command.Execute<VkBotUser>(message, DefaultUser);
            result.Should().BeOfType<FailedResult>();
            result.Message.Should().NotBeNullOrEmpty();
        }
    }
}