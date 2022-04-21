using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace Goblin.Narfu.Tests.StudentsSchedule;

public class GetExamsTests : TestBase
{
    [Fact]
    public async Task GetExams_CorrectGroup_ReturnsLessons()
    {
        var exams = await Api.Students.GetExams(CorrectGroup);
        var str = exams.ToString();

        exams.Should().NotBeNull();
        exams.Lessons.Should()
             .NotBeNullOrEmpty().And
             .HaveCount(3);
        str.Should()
           .NotBeEmpty().And
           .Contain("В аудитории");
    }
}