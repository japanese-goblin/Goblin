using System.IO;
using System.Threading.Tasks;
using FluentAssertions;
using Flurl.Http.Testing;
using Xunit;

namespace Goblin.Narfu.Tests.TeachersSchedule;

public class GetLimitedScheduleTests : TestBase
{
    [Fact]
    public async Task GetLimitedSchedule_CorrectTeacherId_ReturnsLessons()
    {
        using var http = new HttpTest();
        http.RespondWith(await File.ReadAllTextAsync(TeachersSchedulePath));

        var lessons = await Api.Teachers.GetLimitedSchedule(CorrectGroup, 12);
        var str = lessons.ToString();

        lessons.Should().NotBeNull();
        lessons.Lessons.Should()
               .NotBeNullOrEmpty().And
               .HaveCount(12);
        str.Should().NotBeEmpty();
    }
}