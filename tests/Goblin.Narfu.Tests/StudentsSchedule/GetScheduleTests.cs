using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
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
            var correctEndDate = new DateTime(2040, 01, 23, 14, 50, 0);
            var correctStartDate = new DateTime(2040, 01, 23, 13, 15, 0);
            using var http = new HttpTest();
            http.RespondWith(await File.ReadAllTextAsync(StudentsSchedulePath));

            var lessons = await Api.Students.GetSchedule(CorrectGroup);
            var first = lessons.First();

            lessons.Should()
                   .NotBeNullOrEmpty().And
                   .HaveCount(5);
            first.Address.Should().Be("А-НСД22");
            first.Auditory.Should().Be("2212 ЦДЗ");
            first.Groups.Should().Be("271901, 271902, 271903, 271905, 271909");
            first.Id.Should().Be("eb1f309b-6ccc-43d9-9cbc-fb98a85997d2");
            first.Name.Should().Be("Инженерная графика");
            first.Number.Should().Be(5);
            first.Teacher.Should().Be("Пономарева Наталья Геннадьевна");
            first.Type.Should().Be("Консультация");
            first.EndTime.ToUniversalTime().Should().Be(correctEndDate);
            first.StartTime.ToUniversalTime().Should().Be(correctStartDate);
            first.StartEndTime.Should().Be("16:15-17:50");
        }

        [Fact]
        public async Task GetSchedule_SiteIsDown_ThrowsException()
        {
            using var http = new HttpTest();
            http.RespondWith(string.Empty, (int) HttpStatusCode.NotFound);

            Func<Task> func = async () => await Api.Students.GetSchedule(CorrectGroup);

            await func.Should().ThrowAsync<FlurlHttpException>();
        }
    }
}