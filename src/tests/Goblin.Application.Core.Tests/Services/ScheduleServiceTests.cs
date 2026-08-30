using FluentAssertions;
using Goblin.Application.Core.Services;
using Goblin.Narfu.Abstractions;
using Goblin.Narfu.Models;
using Goblin.Narfu.ViewModels;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using Xunit;

namespace Goblin.Application.Core.Tests.Services;

public class ScheduleServiceTests : TestBase
{
    private static ScheduleService GetService(INarfuApi narfuApi)
    {
        var distributedCache = Substitute.For<IDistributedCache>();
        distributedCache.GetAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
                        .Returns(Task.FromResult<byte[]?>(null));

        return new ScheduleService(narfuApi,
                                   distributedCache,
                                   TimeProvider.System,
                                   NullLogger<ScheduleService>.Instance);
    }

    private static INarfuApi GetNarfuApi(bool response = true)
    {
        var mock = Substitute.For<INarfuApi>();
        mock.Students.GetGroupByRealId(Arg.Any<int>())
            .Returns(response ? new Group("name", 1, 1) : null);
        mock.Students.GetScheduleAtDate(Arg.Any<int>(), Arg.Any<DateTime>())
            .Returns(new LessonsViewModel(new List<Lesson>(), DateTime.Today));
        return mock;
    }

    [Fact]
    public async Task ShouldReturnSuccessfulResult()
    {
        var service = GetService(GetNarfuApi());

        var result = await service.GetSchedule(DefaultUser.NarfuGroup!.Value, DateTime.Today);

        result.IsSuccessful.Should().BeTrue();
        result.Message.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task ShouldReturnFailedResult_Because_UserGroupIsZero()
    {
        DefaultUser.SetNarfuGroup(0);
        var service = GetService(GetNarfuApi(false));

        var result = await service.GetSchedule(DefaultUser.NarfuGroup!.Value, DateTime.Today);

        result.IsSuccessful.Should().BeFalse();
        result.Message.Should().NotBeNullOrWhiteSpace();
    }
}
