using FluentAssertions;
using Goblin.Application.Core.Abstractions;
using Goblin.Application.Core.Commands.Merged;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Xunit;

namespace Goblin.Application.Core.Tests.Commands.Merged;

public class ExamsCommandTests : TestBase
{
    private static IScheduleService GetScheduleService()
    {
        var mock = Substitute.For<IScheduleService>();
        mock.GetExams(Arg.Any<int>())
            .Returns(CommandExecutionResult.Success("exams"));
        return mock;
    }

    private static IScheduleService GetScheduleServiceWithHttpException()
    {
        var mock = Substitute.For<IScheduleService>();
        mock.GetExams(Arg.Any<int>())
            .ThrowsAsync(new HttpRequestException());
        return mock;
    }

    private static IScheduleService GetScheduleServiceWithException()
    {
        var mock = Substitute.For<IScheduleService>();
        mock.GetExams(Arg.Any<int>())
            .ThrowsAsync(new Exception());
        return mock;
    }

    [Fact]
    public async Task ShouldReturnSuccessfulResult()
    {
        var command = new ExamsCommand(GetScheduleService(), Substitute.For<ILogger<ExamsCommand>>());
        var message = GenerateMessage(DefaultUser.ConsumerId, DefaultUser.ConsumerId, command.Aliases[0]);

        var result = await command.Execute(message, DefaultUser);
        result.IsSuccessful.Should().BeTrue();
        result.Message.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task ShouldReturnFailedResult_Because_UserGroupIsZero()
    {
        DefaultUser.SetNarfuGroup(null);
        var command = new ExamsCommand(GetScheduleService(), Substitute.For<ILogger<ExamsCommand>>());
        var message = GenerateMessage(DefaultUser.ConsumerId, DefaultUser.ConsumerId, command.Aliases[0]);

        var result = await command.Execute(message, DefaultUser);
        result.IsSuccessful.Should().BeFalse();
        result.Message.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task ShouldReturnFailedResult_Because_SiteIsUnavailable()
    {
        var command = new ExamsCommand(GetScheduleServiceWithHttpException(), Substitute.For<ILogger<ExamsCommand>>());
        var message = GenerateMessage(DefaultUser.ConsumerId, DefaultUser.ConsumerId, command.Aliases[0]);

        var result = await command.Execute(message, DefaultUser);
        result.IsSuccessful.Should().BeFalse();
        result.Message.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task ShouldReturnFailedResult_Because_UnknownError()
    {
        var command = new ExamsCommand(GetScheduleServiceWithException(), Substitute.For<ILogger<ExamsCommand>>());
        var message = GenerateMessage(DefaultUser.ConsumerId, DefaultUser.ConsumerId, command.Aliases[0]);

        var result = await command.Execute(message, DefaultUser);
        result.IsSuccessful.Should().BeFalse();
        result.Message.Should().NotBeNullOrEmpty();
    }
}
