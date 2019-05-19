using System.IO;
using System.Net.Http;
using Flurl.Http.Testing;
using Xunit;

namespace Narfu.Tests
{
    public class TeachersSchedule
    {
        [Fact]
        public async void GetSchedule_CorrectId_Lessons()
        {
            using(var httpTest = new HttpTest())
            {
                httpTest.RespondWith(File.ReadAllText("sched/teachersSchedule.txt"));

                var result = await new NarfuService().Teachers.GetSchedule(22585);

                httpTest.ShouldHaveCalled(Constants.EndPoint)
                        .WithVerb(HttpMethod.Get)
                        .WithQueryParamValue("lecturer", 22585)
                        .Times(1);

                Assert.False(result.IsError);
                Assert.NotEmpty(result.Lessons);
            }
        }

        [Theory]
        [InlineData(22913)]
        [InlineData(22914)]
        public async void GetScheduleAsString_CorrectId_String(int id)
        {
            using(var httpTest = new HttpTest())
            {
                httpTest.RespondWith(File.ReadAllText("sched/teachersSchedule.txt"));

                var result = await new NarfuService().Teachers.GetScheduleAsString(id);
                Assert.NotEmpty(result);
                Assert.Contains("Расписание пар у", result);
            }
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        public async void GetScheduleAsString_IncorrectId_String(int id)
        {
            using(var httpTest = new HttpTest())
            {
                httpTest.RespondWith(File.ReadAllText("sched/teachersSchedule.txt"));

                var result = await new NarfuService().Teachers.GetScheduleAsString(id);
                Assert.NotEmpty(result);
                Assert.Contains("Ошибка", result);
            }
        }

        [Theory]
        [InlineData("Богданов")]
        [InlineData("Деменков")]
        public void FindByName_CorrectName_String(string name)
        {
            var result = new NarfuService().Teachers.FindByName(name);

            Assert.NotEmpty(result);
        }

        [Theory]
        [InlineData("ghj")]
        [InlineData("tyui")]
        public void FindByName_IncorrectName_String(string name)
        {
            var result = new NarfuService().Teachers.FindByName(name);

            Assert.Empty(result);
        }

        [Theory]
        [InlineData(22585)]
        [InlineData(22586)]
        public void FindById_CorrectId_String(int id)
        {
            var result = new NarfuService().Teachers.FindById(id);

            Assert.True(result);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        public void FindById_IncorrectId_String(int id)
        {
            var result = new NarfuService().Teachers.FindById(id);

            Assert.False(result);
        }
    }
}