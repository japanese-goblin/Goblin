using System.Threading.Tasks;
using FluentAssertions;
using Goblin.Application.Core.Commands.Merged;
using Goblin.Application.Core.Results.Success;
using Xunit;

namespace Goblin.Application.Core.Tests.Commands.Merged;

public class GetRemindsCommandTests : TestBase
{
    [Fact]
    public async Task ShouldReturnSuccessfulResult()
    {
        var command = new GetRemindsCommand(ApplicationContext);
        var message = GenerateMessage(DefaultUserWithMaxReminds.Id, DefaultUserWithMaxReminds.Id, command.Aliases[0]);

        var result = await command.Execute(message, DefaultUserWithMaxReminds);
        result.Should().BeOfType<SuccessfulResult>();
        result.Message.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task ShouldReturnSuccessfulResult_Because_RemindsIsZero()
    {
        var command = new GetRemindsCommand(ApplicationContext);
        var message = GenerateMessage(DefaultUser.Id, DefaultUser.Id, command.Aliases[0]);

        var result = await command.Execute(message, DefaultUser);
        result.Should().BeOfType<SuccessfulResult>();
        result.Message.Should().NotBeNullOrEmpty();
    }
}