using FluentAssertions;
using Goblin.Application.Core.Commands.Merged;
using Goblin.Application.Core.Results.Failed;
using Goblin.Application.Core.Results.Success;
using Goblin.Narfu.Abstractions;
using Goblin.Narfu.Models;
using Goblin.Narfu.ViewModels;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Xunit;

namespace Goblin.Application.Core.Tests.Commands.Merged;

public class ExamsCommandTests : TestBase
{
    private static INarfuApi GetNarfuApi()
    {
        var mock = Substitute.For<INarfuApi>();
        mock.Students.GetExams(Arg.Any<int>())
            .Returns(new ExamsViewModel(new List<Lesson>(), DateTime.Today));
        return mock;
    }

    private static INarfuApi GetNarfuApiWithHttpException()
    {
        var mock = Substitute.For<INarfuApi>();
        mock.Students.GetExams(Arg.Any<int>())
            .ThrowsAsync(new HttpRequestException());
        return mock;
    }

    private static INarfuApi GetNarfuApiWithException()
    {
        var mock = Substitute.For<INarfuApi>();
        mock.Students.GetExams(Arg.Any<int>())
            .ThrowsAsync(new Exception());
        return mock;
    }

    [Fact]
    public async Task ShouldReturnSuccessfulResult()
    {
        var command = new ExamsCommand(GetNarfuApi(), Substitute.For<ILogger<ExamsCommand>>());
        var message = GenerateMessage(DefaultUser.Id, DefaultUser.Id, command.Aliases[0]);

        var result = await command.Execute(message, DefaultUser);
        result.Should().BeOfType<SuccessfulResult>();
        result.Message.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task ShouldReturnFailedResult_Because_UserGroupIsZero()
    {
        DefaultUser.SetNarfuGroup(0);
        var command = new ExamsCommand(GetNarfuApi(), Substitute.For<ILogger<ExamsCommand>>());
        var message = GenerateMessage(DefaultUser.Id, DefaultUser.Id, command.Aliases[0]);

        var result = await command.Execute(message, DefaultUser);
        result.Should().BeOfType<FailedResult>();
        result.Message.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task ShouldReturnFailedResult_Because_SiteIsUnavailable()
    {
        var command = new ExamsCommand(GetNarfuApiWithHttpException(), Substitute.For<ILogger<ExamsCommand>>());
        var message = GenerateMessage(DefaultUser.Id, DefaultUser.Id, command.Aliases[0]);

        var result = await command.Execute(message, DefaultUser);
        result.Should().BeOfType<FailedResult>();
        result.Message.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task ShouldReturnFailedResult_Because_UnknownError()
    {
        var command = new ExamsCommand(GetNarfuApiWithException(), Substitute.For<ILogger<ExamsCommand>>());
        var message = GenerateMessage(DefaultUser.Id, DefaultUser.Id, command.Aliases[0]);

        var result = await command.Execute(message, DefaultUser);
        result.Should().BeOfType<FailedResult>();
        result.Message.Should().NotBeNullOrEmpty();
    }
}