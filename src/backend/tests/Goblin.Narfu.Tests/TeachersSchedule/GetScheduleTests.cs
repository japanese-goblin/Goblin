using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace Goblin.Narfu.Tests.TeachersSchedule;

public class GetScheduleTests : TestBase
{
    [Fact]
    public async Task GetSchedule_CorrectId_ReturnsLessons()
    {
        var lessons = await Api.Teachers.GetSchedule(CorrectTeacherId);

        lessons.ToArray().Should()
               .NotBeNullOrEmpty().And
               .HaveCount(27);
    }

    
    [Fact]
    public async Task GetSchedule_IncorrectId_ReturnsLessons()
    {
        Func<Task> func = async () => await Api.Teachers.GetSchedule(5);
    
        await func.Should().ThrowAsync<HttpRequestException>();
    }
}