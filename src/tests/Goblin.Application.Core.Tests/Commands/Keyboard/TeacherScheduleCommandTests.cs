using FluentAssertions;
using Goblin.Application.Core.Commands.Keyboard;
using Goblin.Narfu.Abstractions;
using Goblin.Narfu.Models;
using Goblin.Narfu.ViewModels;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Xunit;

namespace Goblin.Application.Core.Tests.Commands.Keyboard;

public class TeacherScheduleCommandTests : TestBase
{
    private static INarfuApi GetNarfuApi()
    {
        var mockApi = Substitute.For<INarfuApi>();
        mockApi.Teachers.GetLimitedSchedule(Arg.Any<int>(), Arg.Any<int>())
               .Returns(new TeacherLessonsViewModel(new List<Lesson>(), DateTime.Today));
        return mockApi;
    }

    private static INarfuApi GetNarfuApiWithException()
    {
        var mockApi = Substitute.For<INarfuApi>();
        mockApi.Teachers.GetLimitedSchedule(Arg.Any<int>(), Arg.Any<int>())
               .ThrowsAsync(new Exception());
        return mockApi;
    }

    private static INarfuApi GetNarfuApiWithHttpException()
    {
        var mockApi = Substitute.For<INarfuApi>();
        mockApi.Teachers.GetLimitedSchedule(Arg.Any<int>(), Arg.Any<int>())
               .ThrowsAsync(new HttpRequestException());
        return mockApi;
    }

    [Fact]
    public async Task ShouldReturnSuccessfulResult()
    {
        var command = new TeacherScheduleCommand(GetNarfuApi());
        var message = GenerateMessageWithPayload(DefaultUser.Id, DefaultUser.Id, command.Trigger, "12345");

        var result = await command.Execute(message, DefaultUser);
        result.IsSuccessful.Should().BeTrue();
        result.Message.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task ShouldReturnFailedResult_Because_UnknownError()
    {
        var command = new TeacherScheduleCommand(GetNarfuApiWithException());
        var message = GenerateMessageWithPayload(DefaultUser.Id, DefaultUser.Id, command.Trigger, "12345");

        var result = await command.Execute(message, DefaultUser);
        result.IsSuccessful.Should().BeFalse();
        result.Message.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task ShouldReturnFailedResult_Because_SiteIsUnavailable()
    {
        var command = new TeacherScheduleCommand(GetNarfuApiWithHttpException());
        var message = GenerateMessageWithPayload(DefaultUser.Id, DefaultUser.Id, command.Trigger, "12345");

        var result = await command.Execute(message, DefaultUser);
        result.IsSuccessful.Should().BeFalse();
        result.Message.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task ShouldReturnFailedResult_Because_DictionaryKeyIsIncorrect()
    {
        var command = new TeacherScheduleCommand(GetNarfuApi());
        var message = GenerateMessageWithPayload(DefaultUser.Id, DefaultUser.Id, "key", "12345");

        var result = await command.Execute(message, DefaultUser);
        result.IsSuccessful.Should().BeFalse();
        result.Message.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task ShouldReturnFailedResult_Because_DictionaryValueIsNotInteger()
    {
        var command = new TeacherScheduleCommand(GetNarfuApi());
        var message = GenerateMessageWithPayload(DefaultUser.Id, DefaultUser.Id, command.Trigger, "asd");

        var result = await command.Execute(message, DefaultUser);
        result.IsSuccessful.Should().BeFalse();
        result.Message.Should().NotBeNullOrEmpty();
    }
}