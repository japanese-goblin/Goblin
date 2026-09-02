using FluentAssertions;
using Goblin.Application.Core.Commands.Merged;
using Xunit;

namespace Goblin.Application.Core.Tests.Commands.Merged;

public class HelpCommandTests : TestBase
{
    [Fact]
    public async Task ShouldReturnSuccessfulResult()
    {
        var command = new HelpCommand();
        var message = GenerateMessage(DefaultUser.ConsumerId, DefaultUser.ConsumerId, command.Aliases[0]);

        var result = await command.Execute(message, DefaultUser);
        result.IsSuccessful.Should().BeTrue();
        result.Message.Should().NotBeNullOrEmpty();
    }
}
