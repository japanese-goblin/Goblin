using System;
using FluentAssertions;
using Xunit;

namespace Goblin.Narfu.Tests.StudentsSchedule
{
    public class GenerateScheduleLinkTests : TestBase
    {
        [Fact]
        public void GenerateScheduleLink_Https_ReturnsLink()
        {
            var todayDate = DateTime.Today.ToString("dd.MM.yyyy");

            var link = Api.Students.GenerateScheduleLink(CorrectGroup);

            link.Should().Be($"https://ruz.narfu.ru/?icalendar&oid=12289&cod={CorrectGroup}&from={todayDate}");
        }

        [Fact]
        public void GenerateScheduleLink_Webcal_ReturnsLink()
        {
            var todayDate = DateTime.Today.ToString("dd.MM.yyyy");

            var link = Api.Students.GenerateScheduleLink(CorrectGroup, true);

            link.Should().Be($"webcal://ruz.narfu.ru/?icalendar&oid=12289&cod={CorrectGroup}&from={todayDate}");
        }
    }
}