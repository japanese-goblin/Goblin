using System.IO;
using System.Linq;
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

    //
    // [Fact]
    // public async Task GetSchedule_IncorrectId_ReturnsLessons()
    // {
    //     using var http = new HttpTest();
    //     http.RespondWith(string.Empty, (int) HttpStatusCode.NotFound);
    //
    //     Func<Task> func = async () => await Api.Teachers.GetSchedule(CorrectTeacherId);
    //
    //     await func.Should().ThrowAsync<FlurlHttpException>();
    // }
}