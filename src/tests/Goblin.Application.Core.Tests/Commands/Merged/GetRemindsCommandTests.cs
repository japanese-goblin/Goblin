using FluentAssertions;
using Goblin.Application.Core.Commands.Merged;
using Xunit;

namespace Goblin.Application.Core.Tests.Commands.Merged;

public class GetRemindsCommandTests : TestBase
{
    [Fact]
    public async Task ShouldReturnSuccessfulResult()
    {
        var command = new GetRemindsCommand(GetDbContext());
        var message = GenerateMessage(DefaultUserWithMaxReminds.Id, DefaultUserWithMaxReminds.Id, command.Aliases[0]);

        var result = await command.Execute(message, DefaultUserWithMaxReminds);
        result.IsSuccessful.Should().BeTrue();
        result.Message.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task ShouldReturnSuccessfulResult_Because_RemindsIsZero()
    {
        var command = new GetRemindsCommand(GetDbContext());
        var message = GenerateMessage(DefaultUser.Id, DefaultUser.Id, command.Aliases[0]);

        var result = await command.Execute(message, DefaultUser);
        result.IsSuccessful.Should().BeTrue();
        result.Message.Should().NotBeNullOrEmpty();
    }
}