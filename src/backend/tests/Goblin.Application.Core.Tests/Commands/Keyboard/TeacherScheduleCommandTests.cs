using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Flurl;
using Flurl.Http;
using Goblin.Application.Core.Commands.Keyboard;
using Goblin.Application.Core.Results.Failed;
using Goblin.Application.Core.Results.Success;
using Goblin.Narfu.Abstractions;
using Goblin.Narfu.Models;
using Goblin.Narfu.ViewModels;
using Moq;
using Xunit;

namespace Goblin.Application.Core.Tests.Commands.Keyboard;

public class TeacherScheduleCommandTests : TestBase
{
    private INarfuApi GetNarfuApi()
    {
        var mockApi = new Mock<INarfuApi>();
        mockApi.Setup(x => x.Teachers.GetLimitedSchedule(It.IsAny<int>(), It.IsAny<int>()))
               .ReturnsAsync(new TeacherLessonsViewModel(new List<Lesson>(), DateTime.Today));

        return mockApi.Object;
    }

    private INarfuApi GetNarfuApiWithException()
    {
        var mockApi = new Mock<INarfuApi>();
        mockApi.Setup(x => x.Teachers.GetLimitedSchedule(It.IsAny<int>(), It.IsAny<int>()))
               .ThrowsAsync(new Exception());

        return mockApi.Object;
    }

    private INarfuApi GetNarfuApiWithFlurlException()
    {
        const string endPoint = "https://localhost";
        var mockApi = new Mock<INarfuApi>();
        mockApi.Setup(x => x.Teachers.GetLimitedSchedule(It.IsAny<int>(), It.IsAny<int>()))
               .ThrowsAsync(new FlurlHttpException(new FlurlCall()
               {
                   Request = new FlurlRequest(new Url(endPoint)),
                   HttpRequestMessage = new HttpRequestMessage(HttpMethod.Get, endPoint)
               }));

        return mockApi.Object;
    }

    [Fact]
    public async Task ShouldReturnSuccessfulResult()
    {
        var command = new TeacherScheduleCommand(GetNarfuApi());
        var message = GenerateMessageWithPayload(DefaultUser.Id, DefaultUser.Id, command.Trigger, "12345");

        var result = await command.Execute(message, DefaultUser);
        result.Should().BeOfType<SuccessfulResult>();
        result.Message.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task ShouldReturnFailedResult_Because_UnknownError()
    {
        var command = new TeacherScheduleCommand(GetNarfuApiWithException());
        var message = GenerateMessageWithPayload(DefaultUser.Id, DefaultUser.Id, command.Trigger, "12345");

        var result = await command.Execute(message, DefaultUser);
        result.Should().BeOfType<FailedResult>();
        result.Message.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task ShouldReturnFailedResult_Because_SiteIsUnavailable()
    {
        var command = new TeacherScheduleCommand(GetNarfuApiWithFlurlException());
        var message = GenerateMessageWithPayload(DefaultUser.Id, DefaultUser.Id, command.Trigger, "12345");

        var result = await command.Execute(message, DefaultUser);
        result.Should().BeOfType<FailedResult>();
        result.Message.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task ShouldReturnFailedResult_Because_DictionaryKeyIsIncorrect()
    {
        var command = new TeacherScheduleCommand(GetNarfuApi());
        var message = GenerateMessageWithPayload(DefaultUser.Id, DefaultUser.Id, "key", "12345");

        var result = await command.Execute(message, DefaultUser);
        result.Should().BeOfType<FailedResult>();
        result.Message.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task ShouldReturnFailedResult_Because_DictionaryValueIsNotInteger()
    {
        var command = new TeacherScheduleCommand(GetNarfuApi());
        var message = GenerateMessageWithPayload(DefaultUser.Id, DefaultUser.Id, command.Trigger, "asd");

        var result = await command.Execute(message, DefaultUser);
        result.Should().BeOfType<FailedResult>();
        result.Message.Should().NotBeNullOrEmpty();
    }
}