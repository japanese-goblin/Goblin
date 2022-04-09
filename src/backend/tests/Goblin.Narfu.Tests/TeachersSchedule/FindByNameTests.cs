using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Flurl.Http.Testing;
using Xunit;

namespace Goblin.Narfu.Tests.TeachersSchedule;

public class FindByNameTests : TestBase
{
    [Fact]
    public async Task FindByName_CorrectName_ReturnsTeachers()
    {
        using var http = new HttpTest();
        http.RespondWith(await File.ReadAllTextAsync(FindByNamePath));

        var teachers = await Api.Teachers.FindByName("Абрамова");
        var first = teachers.First();

        teachers.Should()
                .NotBeNullOrEmpty().And
                .HaveCount(1);
        first.Depart.Should().Be("Кафедра информационных систем и технологий");
        first.Id.Should().Be(31261);
        first.Name.Should().Be("Абрамова Любовь Валерьевна");
    }
}