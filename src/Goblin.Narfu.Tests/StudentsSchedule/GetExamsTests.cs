using System.IO;
using System.Linq;
using System.Threading.Tasks;
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
            http.RespondWith(File.ReadAllText(StudentsSchedulePath));

            var exams = (await Api.Students.GetExams(CorrectGroup)).Lessons.ToArray();

            Assert.NotEmpty(exams);
            Assert.Equal(3, exams.Length);
        }
    }
}