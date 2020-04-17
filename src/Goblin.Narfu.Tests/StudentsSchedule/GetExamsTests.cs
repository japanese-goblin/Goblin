using System.IO;
using System.Threading.Tasks;
using FluentAssertions;
using Flurl.Http.Testing;
using Xunit;

namespace Goblin.Narfu.Tests.StudentsSchedule
{
    public class GetExamsTests : TestBase
    {
        [Fact]
        public async Task GetExams_CorrectGroup_ReturnsLessons()
        {
            using var http = new HttpTest();
            http.RespondWith(await File.ReadAllTextAsync(StudentsSchedulePath));

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
}