using FluentAssertions;
using Xunit;

namespace Goblin.Narfu.Tests.StudentsSchedule;

public class GetScheduleAtDateTests : TestBase
{
    [Fact(Skip = "Доработать с NSubstitute")]
    public async Task GetScheduleAtDate_CorrectGroupAndDate_ReturnsLessons()
    {
        var lessons = await Api.Students.GetScheduleAtDate(CorrectGroup, CorrectDate);
        var str = lessons.ToString();

        lessons.Should().NotBeNull();
        lessons.Lessons.Should()
               .NotBeNull().And
               .HaveCount(1);
        str.Should().NotBeEmpty();
        str.Should().Contain("Инженерная графика");
    }

    [Fact(Skip = "Доработать с NSubstitute")]
    public async Task GetScheduleAtDate_IncorrectDate_ReturnsLessons()
    {
        var lessons = await Api.Students.GetScheduleAtDate(CorrectGroup, IncorrectDate);
        var str = lessons.ToString();

        lessons.Should().NotBeNull();
        lessons.Lessons.Should().BeEmpty();
        str.Should().Contain("отсутствует");
    }
}