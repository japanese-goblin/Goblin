using FluentAssertions;
using Goblin.Application.Core.Commands.Text;
using Xunit;

namespace Goblin.Application.Core.Tests.Commands.Text;

public class DebugCommandTests : TestBase
{
    [Fact]
    public async Task ShouldReturnSuccessfulResult()
    {
        var command = new DebugCommand(GetDbContext());
        var text = command.Aliases[0];
        var message = GenerateMessage(DefaultUser.Id, DefaultUser.Id, text);

        var result = await command.Execute(message, DefaultUser);

        result.IsSuccessful.Should().BeTrue();
        result.Message.Should().NotBeNullOrEmpty();
    }
}