using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Goblin.Application.Core.Results.Failed;
using Goblin.Application.Core.Results.Success;
using Goblin.Application.Core.Services;
using Goblin.Narfu.Abstractions;
using Goblin.Narfu.Models;
using Goblin.Narfu.ViewModels;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace Goblin.Application.Core.Tests.Services;

public class ScheduleServiceTests : TestBase
{
    private static INarfuApi GetNarfuApi(bool response = true)
    {
        var mock = Substitute.For<INarfuApi>();
        mock.Students.IsCorrectGroup(Arg.Any<int>())
            .Returns(response);
        mock.Students.GetScheduleAtDate(Arg.Any<int>(), Arg.Any<DateTime>())
            .Returns(new LessonsViewModel(new List<Lesson>(), DateTime.Today));
        return mock;
    }

    [Fact]
    public async Task ShouldReturnSuccessfulResult()
    {
        var service = new ScheduleService(GetNarfuApi(), Substitute.For<ILogger<ScheduleService>>());

        var result = await service.GetSchedule(DefaultUser.NarfuGroup, DateTime.Today);

        result.Should().BeOfType<SuccessfulResult>();
        result.Message.Should().NotBeNullOrWhiteSpace();
        result.Keyboard.Should().NotBeNull();
    }

    [Fact]
    public async Task ShouldReturnFailedResult_Because_UserGroupIsZero()
    {
        DefaultUser.SetNarfuGroup(0);
        var service = new ScheduleService(GetNarfuApi(false), Substitute.For<ILogger<ScheduleService>>());

        var result = await service.GetSchedule(DefaultUser.NarfuGroup, DateTime.Today);

        result.Should().BeOfType<FailedResult>();
        result.Message.Should().NotBeNullOrWhiteSpace();
    }
}