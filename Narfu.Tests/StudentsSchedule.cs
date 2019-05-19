using System;
using System.IO;
using System.Net;
using System.Net.Http;
using Flurl.Http.Testing;
using Xunit;

namespace Narfu.Tests
{
    public class StudentsSchedule
    {
        [Fact]
        public async void GetSchedule_CorrectGroup_Lessons()
        {
            using(var httpTest = new HttpTest())
            {
                httpTest.RespondWith(File.ReadAllText("sched/studentsSchedule.txt"));

                var result = await new NarfuService().Students.GetSchedule(351617);

                httpTest.ShouldHaveCalled(Constants.EndPoint)
                        .WithVerb(HttpMethod.Get)
                        .WithQueryParamValue("cod", 351617)
                        .Times(1);

                Assert.False(result.Error);
                Assert.Equal(HttpStatusCode.OK, result.Code);
                Assert.NotEmpty(result.Lessons);
            }
        }

        [Fact]
        public async void GetScheduleAsStringAtDate_CorrectData_String()
        {
            using(var httpTest = new HttpTest())
            {
                httpTest.RespondWith(File.ReadAllText("sched/studentsSchedule.txt"));

                var result = await new NarfuService().Students.GetScheduleAsStringAtDate(new DateTime(2019, 6, 14), 351617);

                Assert.NotEmpty(result);
                Assert.Contains("Предмет1", result);
            }
        }

        [Fact]
        public async void GetScheduleAsStringAtDate_EmptyDay_String()
        {
            using(var httpTest = new HttpTest())
            {
                httpTest.RespondWith(File.ReadAllText("sched/studentsSchedule.txt"));

                var result = await new NarfuService().Students.GetScheduleAsStringAtDate(new DateTime(2019, 1, 1), 351617);

                Assert.NotEmpty(result);
                Assert.Contains("отсутствует", result);
            }
        }

        [Fact]
        public async void GetExams_CorrectData_Lessons()
        {
            using(var httpTest = new HttpTest())
            {
                httpTest.RespondWith(File.ReadAllText("sched/studentsSchedule.txt"));

                var result = await new NarfuService().Students.GetExams(351617);

                Assert.False(result.Error);
                Assert.Equal(HttpStatusCode.OK, result.Code);
                Assert.NotEmpty(result.Lessons);
            }
        }

        [Fact]
        public async void GetExamsAsString_CorrectData_String()
        {
            using(var httpTest = new HttpTest())
            {
                httpTest.RespondWith(File.ReadAllText("sched/studentsSchedule.txt"));

                var result = await new NarfuService().Students.GetExamsAsString(351617);

                Assert.NotEmpty(result);
                Assert.Contains("Предмет1", result);
            }
        }

        [Theory]
        [InlineData(351617)]
        [InlineData(351618)]
        public void IsCorrectGroup_CorrectGroup_True(int id)
        {
            var result = new NarfuService().Students.IsCorrectGroup(id);
            Assert.True(result);
        }

        [Theory]
        [InlineData(123456)]
        [InlineData(654321)]
        public void IsCorrectGroup_IncorrectGroup_False(int id)
        {
            var result = new NarfuService().Students.IsCorrectGroup(id);
            Assert.False(result);
        }

        [Fact]
        public void GetGroupByRealId_CorrectId_Group()
        {
            var result = new NarfuService().Students.GetGroupByRealId(351617);

            Assert.NotNull(result);
            Assert.Equal(9092, result.SiteId);
        }

        [Fact]
        public void GetGroupBySiteId_CorrectId_Group()
        {
            var result = new NarfuService().Students.GetGroupBySiteId(9092);

            Assert.NotNull(result);
            Assert.Equal(351617, result.RealId);
        }
    }
}
