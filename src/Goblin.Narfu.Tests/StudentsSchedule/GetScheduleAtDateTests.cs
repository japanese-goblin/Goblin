using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Flurl.Http.Testing;
using Xunit;

namespace Goblin.Narfu.Tests.StudentsSchedule
{
    public class GetScheduleAtDateTests : TestBase
    {
        [Fact]
        public async Task GetScheduleAtDate_CorrectGroupAndDate_ReturnsLessons()
        {
            using (var http = new HttpTest())
            {
                http.RespondWith(File.ReadAllText(StudentsSchedulePath));

                var lessons = await Api.Students.GetScheduleAtDate(CorrectGroup, CorrectDate);
                var str = lessons.ToString();

                Assert.NotEmpty(lessons.Lessons);
                Assert.Equal(1, lessons.Lessons.Count());
                Assert.Contains("Инженерная графика", str);
            }
        }
        
        [Fact]
        public async Task GetScheduleAtDate_IncorrectDate_ReturnsLessons()
        {
            using (var http = new HttpTest())
            {
                http.RespondWith(File.ReadAllText(StudentsSchedulePath));

                var lessons = await Api.Students.GetScheduleAtDate(CorrectGroup, IncorrectDate);
                var str = lessons.ToString();

                Assert.Empty(lessons.Lessons.ToArray());
                Assert.Contains("отсутствует", str);
            }
        }
    }
}