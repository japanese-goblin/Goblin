﻿using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Goblin.Application.Core.Commands.Merged;
using Goblin.Application.Core.Results.Failed;
using Goblin.Application.Core.Results.Success;
using Goblin.Narfu.Abstractions;
using Goblin.Narfu.Models;
using Goblin.Narfu.ViewModels;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Goblin.Application.Core.Tests.Commands.Merged;

public class ExamsCommandTests : TestBase
{
    private INarfuApi GetNarfuApi()
    {
        var mockApi = new Mock<INarfuApi>();
        mockApi.Setup(x => x.Students.GetExams(It.IsAny<int>()))
               .ReturnsAsync(new ExamsViewModel(new List<Lesson>(), DateTime.Today));

        return mockApi.Object;
    }

    private INarfuApi GetNarfuApiWithHttpException()
    {
        var mockApi = new Mock<INarfuApi>();
        mockApi.Setup(x => x.Students.GetExams(It.IsAny<int>()))
               .ThrowsAsync(new HttpRequestException());

        return mockApi.Object;
    }

    private INarfuApi GetNarfuApiWithException()
    {
        var mockApi = new Mock<INarfuApi>();
        mockApi.Setup(x => x.Students.GetExams(It.IsAny<int>()))
               .ThrowsAsync(new Exception());

        return mockApi.Object;
    }

    [Fact]
    public async Task ShouldReturnSuccessfulResult()
    {
        var command = new ExamsCommand(GetNarfuApi(), Mock.Of<ILogger<ExamsCommand>>());
        var message = GenerateMessage(DefaultUser.Id, DefaultUser.Id, command.Aliases[0]);

        var result = await command.Execute(message, DefaultUser);
        result.Should().BeOfType<SuccessfulResult>();
        result.Message.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task ShouldReturnFailedResult_Because_UserGroupIsZero()
    {
        DefaultUser.SetNarfuGroup(0);
        var command = new ExamsCommand(GetNarfuApi(), Mock.Of<ILogger<ExamsCommand>>());
        var message = GenerateMessage(DefaultUser.Id, DefaultUser.Id, command.Aliases[0]);

        var result = await command.Execute(message, DefaultUser);
        result.Should().BeOfType<FailedResult>();
        result.Message.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task ShouldReturnFailedResult_Because_SiteIsUnavailable()
    {
        var command = new ExamsCommand(GetNarfuApiWithHttpException(), Mock.Of<ILogger<ExamsCommand>>());
        var message = GenerateMessage(DefaultUser.Id, DefaultUser.Id, command.Aliases[0]);

        var result = await command.Execute(message, DefaultUser);
        result.Should().BeOfType<FailedResult>();
        result.Message.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task ShouldReturnFailedResult_Because_UnknownError()
    {
        var command = new ExamsCommand(GetNarfuApiWithException(), Mock.Of<ILogger<ExamsCommand>>());
        var message = GenerateMessage(DefaultUser.Id, DefaultUser.Id, command.Aliases[0]);

        var result = await command.Execute(message, DefaultUser);
        result.Should().BeOfType<FailedResult>();
        result.Message.Should().NotBeNullOrEmpty();
    }
}