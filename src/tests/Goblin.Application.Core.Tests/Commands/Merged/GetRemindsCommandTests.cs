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
        var message = GenerateMessage(DefaultUserWithMaxReminds.ConsumerId, DefaultUserWithMaxReminds.ConsumerId, command.Aliases[0]);

        var result = await command.Execute(message, DefaultUserWithMaxReminds);
        result.IsSuccessful.Should().BeTrue();
        result.Message.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task ShouldReturnSuccessfulResult_Because_RemindsIsZero()
    {
        var command = new GetRemindsCommand(GetDbContext());
        var message = GenerateMessage(DefaultUser.ConsumerId, DefaultUser.ConsumerId, command.Aliases[0]);

        var result = await command.Execute(message, DefaultUser);
        result.IsSuccessful.Should().BeTrue();
        result.Message.Should().NotBeNullOrEmpty();
    }
}