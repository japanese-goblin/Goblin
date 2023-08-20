using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace Goblin.Narfu.Tests.TeachersSchedule;

public class FindByNameTests : TestBase
{
    [Fact(Skip = "Доработать с NSubstitute")]
    public async Task FindByName_CorrectName_ReturnsTeachers()
    {
        var teachers = await Api.Teachers.FindByName(TeacherName);
        var first = teachers.First();

        teachers.Should()
                .NotBeNullOrEmpty().And
                .HaveCount(1);
        first.Depart.Should().Be("Кафедра информационных систем и технологий");
        first.Id.Should().Be(31261);
        first.Name.Should().Be("Абрамова Любовь Валерьевна");
    }
}