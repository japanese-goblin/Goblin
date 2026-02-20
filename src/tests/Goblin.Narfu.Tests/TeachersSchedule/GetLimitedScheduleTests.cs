using FluentAssertions;
using Xunit;

namespace Goblin.Narfu.Tests.TeachersSchedule;

public class GetLimitedScheduleTests : TestBase
{
    [Fact(Skip = "Доработать с NSubstitute")]
    public async Task GetLimitedSchedule_CorrectTeacherId_ReturnsLessons()
    {
        var lessons = await Api.Teachers.GetLimitedSchedule(CorrectTeacherId, 12);
        var str = lessons.ToString();

        lessons.Should().NotBeNull();
        lessons.Lessons.Should()
               .NotBeNullOrEmpty().And
               .HaveCount(12);
        str.Should().NotBeEmpty();
    }
}