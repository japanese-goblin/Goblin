using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Flurl.Http;
using Flurl.Http.Testing;
using Xunit;

namespace Goblin.Narfu.Tests.TeachersSchedule
{
    public class GetScheduleTests : TestBase
    {
        [Fact]
        public async Task GetSchedule_CorrectId_ReturnsLessons()
        {
            using (var http = new HttpTest())
            {
                http.RespondWith(File.ReadAllText(TeachersSchedulePath));

                var lessons = (await Api.Teachers.GetSchedule(CorrectTeacherId)).ToArray();
                
                Assert.NotEmpty(lessons);
                Assert.Equal(27, lessons.Length);
            }
        }
        
        [Fact]
        public async Task GetSchedule_IncorrectId_ReturnsLessons()
        {
            using (var http = new HttpTest())
            {
                http.RespondWith(string.Empty, (int)HttpStatusCode.NotFound);

                await Assert.ThrowsAsync<FlurlHttpException>(async () => await Api.Teachers.GetSchedule(CorrectTeacherId));
            }
        }
    }
}