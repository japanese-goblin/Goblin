using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Goblin.Application.Core.Commands.Text;
using Goblin.Application.Core.Results.Failed;
using Goblin.Application.Core.Results.Success;
using Goblin.Narfu.Abstractions;
using Goblin.Narfu.Models;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Xunit;

namespace Goblin.Application.Core.Tests.Commands.Text;

public class FindTeacherCommandTests : TestBase
{
    private static INarfuApi GetNarfuApi(int max = 3)
    {
        var teachers = Enumerable.Range(0, max).Select(x => new Teacher
        {
            Id = x,
            Depart = "depart",
            Name = $"Name #{x}"
        }).ToArray();

        var mockApi = Substitute.For<INarfuApi>();
        mockApi.Teachers.FindByName(Arg.Any<string>())
               .Returns(teachers);
        return mockApi;
    }

    private static INarfuApi GetNarfuApiWithException()
    {
        var mockApi = Substitute.For<INarfuApi>();
        mockApi.Teachers.FindByName(Arg.Any<string>())
               .ThrowsAsync(new Exception());
        return mockApi;
    }

    private static INarfuApi GetNarfuApiWithHttpException()
    {
        var mockApi = Substitute.For<INarfuApi>();
        mockApi.Teachers.FindByName(Arg.Any<string>())
               .ThrowsAsync(new HttpRequestException());
        return mockApi;
    }

    [Fact]
    public async Task ShouldReturnFailedResult_Because_FoundedALotOfTeachers()
    {
        var command = new FindTeacherCommand(GetNarfuApi(10), Substitute.For<ILogger<FindTeacherCommand>>());
        var text = $"{command.Aliases[0]} Петров Пётр Петрович";
        var message = GenerateMessage(DefaultUser.Id, DefaultUser.Id, text);

        var result = await command.Execute(message, DefaultUser);

        result.Should().BeOfType<FailedResult>();
        result.Message.Should().NotBeNullOrEmpty();
        result.Keyboard.Should().BeNull();
    }

    [Fact]
    public async Task ShouldReturnFailedResult_Because_ParameterIsEmpty()
    {
        var command = new FindTeacherCommand(GetNarfuApi(), Substitute.For<ILogger<FindTeacherCommand>>());
        var text = $"{command.Aliases[0]} ";
        var message = GenerateMessage(DefaultUser.Id, DefaultUser.Id, text);

        var result = await command.Execute(message, DefaultUser);

        result.Should().BeOfType<FailedResult>();
        result.Message.Should().NotBeNullOrEmpty();
        result.Keyboard.Should().BeNull();
    }

    [Fact]
    public async Task ShouldReturnFailedResult_Because_SiteIsUnavailable()
    {
        var command = new FindTeacherCommand(GetNarfuApiWithHttpException(), Substitute.For<ILogger<FindTeacherCommand>>());
        var text = $"{command.Aliases[0]} Петров Пётр Петрович";
        var message = GenerateMessage(DefaultUser.Id, DefaultUser.Id, text);

        var result = await command.Execute(message, DefaultUser);

        result.Should().BeOfType<FailedResult>();
        result.Message.Should().NotBeEmpty();
        result.Keyboard.Should().BeNull();
    }

    [Fact]
    public async Task ShouldReturnFailedResult_Because_TeachersNotFound()
    {
        var command = new FindTeacherCommand(GetNarfuApi(0), Substitute.For<ILogger<FindTeacherCommand>>());
        var text = $"{command.Aliases[0]} Петров Пётр Петрович";
        var message = GenerateMessage(DefaultUser.Id, DefaultUser.Id, text);

        var result = await command.Execute(message, DefaultUser);

        result.Should().BeOfType<FailedResult>();
        result.Message.Should().NotBeNullOrEmpty();
        result.Keyboard.Should().BeNull();
    }

    [Fact]
    public async Task ShouldReturnFailedResult_Because_UnknownError()
    {
        var command = new FindTeacherCommand(GetNarfuApiWithException(), Substitute.For<ILogger<FindTeacherCommand>>());
        var text = $"{command.Aliases[0]} Петров Пётр Петрович";
        var message = GenerateMessage(DefaultUser.Id, DefaultUser.Id, text);

        var result = await command.Execute(message, DefaultUser);

        result.Should().BeOfType<FailedResult>();
        result.Message.Should().NotBeEmpty();
        result.Keyboard.Should().BeNull();
    }

    [Fact]
    public async Task ShouldReturnSuccessfulResult()
    {
        var command = new FindTeacherCommand(GetNarfuApi(), Substitute.For<ILogger<FindTeacherCommand>>());
        var text = $"{command.Aliases[0]} Иванов Иван Иванович";
        var message = GenerateMessage(DefaultUser.Id, DefaultUser.Id, text);

        var result = await command.Execute(message, DefaultUser);

        result.Should().BeOfType<SuccessfulResult>();
        result.Message.Should().NotBeNullOrEmpty();
        result.Keyboard.Should().NotBeNull();
        result.Keyboard.Buttons.Should().HaveCount(4);
    }
}