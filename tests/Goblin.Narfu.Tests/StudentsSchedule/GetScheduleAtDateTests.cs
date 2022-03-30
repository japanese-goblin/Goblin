using System.IO;
using System.Threading.Tasks;
using FluentAssertions;
using Flurl.Http.Testing;
using Xunit;

namespace Goblin.Narfu.Tests.StudentsSchedule;

public class GetScheduleAtDateTests : TestBase
{
    [Fact]
    public async Task GetScheduleAtDate_CorrectGroupAndDate_ReturnsLessons()
    {
        using var http = new HttpTest();
        http.RespondWith(await File.ReadAllTextAsync(StudentsSchedulePath));

        var lessons = await Api.Students.GetScheduleAtDate(CorrectGroup, CorrectDate);
        var str = lessons.ToString();

        lessons.Should().NotBeNull();
        lessons.Lessons.Should()
               .NotBeNull().And
               .HaveCount(1);
        str.Should().NotBeEmpty();
        str.Should().Contain("Инженерная графика");
    }

    [Fact]
    public async Task GetScheduleAtDate_IncorrectDate_ReturnsLessons()
    {
        using var http = new HttpTest();
        http.RespondWith(await File.ReadAllTextAsync(StudentsSchedulePath));

        var lessons = await Api.Students.GetScheduleAtDate(CorrectGroup, IncorrectDate);
        var str = lessons.ToString();

        lessons.Should().NotBeNull();
        lessons.Lessons.Should().BeEmpty();
        str.Should().Contain("отсутствует");
    }
}