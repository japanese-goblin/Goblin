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
using Moq;
using Xunit;

namespace Goblin.Application.Core.Tests.Commands.Text;

public class FindTeacherCommandTests : TestBase
{
    private INarfuApi GetNarfuApi(int max = 3)
    {
        var teachers = Enumerable.Range(0, max).Select(x => new Teacher
        {
            Id = x,
            Depart = "depart",
            Name = $"Name #{x}"
        }).ToArray();

        var mockApi = new Mock<INarfuApi>();
        mockApi.Setup(x => x.Teachers.FindByName(It.IsAny<string>()))
               .ReturnsAsync(teachers);

        return mockApi.Object;
    }

    private INarfuApi GetNarfuApiWithException()
    {
        var mockApi = new Mock<INarfuApi>();
        mockApi.Setup(x => x.Teachers.FindByName(It.IsAny<string>()))
               .ThrowsAsync(new Exception());

        return mockApi.Object;
    }

    private INarfuApi GetNarfuApiWithHttpException()
    {
        var mockApi = new Mock<INarfuApi>();
        mockApi.Setup(x => x.Teachers.FindByName(It.IsAny<string>()))
               .ThrowsAsync(new HttpRequestException());

        return mockApi.Object;
    }

    [Fact]
    public async Task ShouldReturnFailedResult_Because_FoundedALotOfTeachers()
    {
        var command = new FindTeacherCommand(GetNarfuApi(10));
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
        var command = new FindTeacherCommand(GetNarfuApi());
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
        var command = new FindTeacherCommand(GetNarfuApiWithHttpException());
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
        var command = new FindTeacherCommand(GetNarfuApi(0));
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
        var command = new FindTeacherCommand(GetNarfuApiWithException());
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
        var command = new FindTeacherCommand(GetNarfuApi());
        var text = $"{command.Aliases[0]} Иванов Иван Иванович";
        var message = GenerateMessage(DefaultUser.Id, DefaultUser.Id, text);

        var result = await command.Execute(message, DefaultUser);

        result.Should().BeOfType<SuccessfulResult>();
        result.Message.Should().NotBeNullOrEmpty();
        result.Keyboard.Should().NotBeNull();
        result.Keyboard.Buttons.Should().HaveCount(4);
    }
}