using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Flurl.Http.Testing;
using Xunit;

namespace Goblin.Narfu.Tests.TeachersSchedule
{
    public class GetLimitedScheduleTests : TestBase
    {
        [Fact]
        public async Task GetLimitedSchedule_CorrectTeacherId_ReturnsLessons()
        {
            using (var http = new HttpTest())
            {
                http.RespondWith(File.ReadAllText(TeachersSchedulePath));

                var lessons = await Api.Teachers.GetLimitedSchedule(CorrectGroup, 12);
                
                Assert.NotEmpty(lessons.Lessons);
                Assert.Equal(12, lessons.Lessons.Count());
            }
        }
    }
}