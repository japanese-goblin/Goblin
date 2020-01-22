using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Flurl.Http;
using Flurl.Http.Testing;
using Xunit;

namespace Goblin.Narfu.Tests.StudentsSchedule
{
    public class GetScheduleTests : TestBase
    {
        [Fact]
        public async Task GetSchedule_CorrectGroup_ReturnsLessons()
        {
            using (var http = new HttpTest())
            {
                http.RespondWith(File.ReadAllText(StudentsSchedulePath));

                var lessons = (await Api.Students.GetSchedule(CorrectGroup)).ToArray();
                var first = lessons.First();
                
                Assert.NotEmpty(lessons);
                Assert.Equal(5, lessons.Length);
                
                Assert.Equal("Инженерная графика", first.Name);
                Assert.Equal("Консультация", first.Type);
                Assert.Equal("Пономарева Наталья Геннадьевна", first.Teacher);
                Assert.Equal("А-НСД22", first.Address);
                Assert.Equal("2212 ЦДЗ", first.Auditory);
                Assert.Equal("271901, 271902, 271903, 271905, 271909", first.Groups);
                Assert.Equal(5, first.Number);
                Assert.Equal("5) 16:15-17:50", first.StartEndTime);
                Assert.Equal(new DateTime(2020, 01, 23, 16, 15, 0), first.StartTime);
                Assert.Equal(new DateTime(2020, 01, 23, 17, 50, 0), first.EndTime);
            }
        }

        [Fact]
        public async Task GetSchedule_SiteIsDown_ThrowsException()
        {
            using (var http = new HttpTest())
            {
                http.RespondWith(string.Empty, (int)HttpStatusCode.NotFound);

                await Assert.ThrowsAsync<FlurlHttpException>(async () => await Api.Students.GetSchedule(CorrectGroup));
            }
        }
    }
}