using System;
using System.Threading.Tasks;
using Xunit;

namespace Tests.Schedule
{
    public class StudentsSchedule
    {
        private const int CorrectGroup = 351617;
        [Fact]
        public async Task GetSchedule_Correct()
        {
            var (isError, _) = await Narfu.StudentsSchedule.GetSchedule(CorrectGroup);
            Assert.False(isError, "Сайт сломался?");
        }

        [Fact]
        public async Task GetScheduleAtDate_Correct()
        {
            var result = await Narfu.StudentsSchedule.GetScheduleAtDate(DateTime.Now, CorrectGroup);
            Assert.True(result.ToLower().Contains("расписание"), "Сайт сломался?");
        }

        [Fact]
        public async Task GetExams_Correct()
        {
            var result = await Narfu.StudentsSchedule.GetExams(CorrectGroup);
            Assert.True(result.Contains("экзаменов"), "Сайт сломался?");
        }

        [Fact]
        public void IsCorrectGroup_Valid_True()
        {
            var result = Narfu.StudentsSchedule.IsCorrectGroup(CorrectGroup);
            Assert.True(result);
        }

        [Fact]
        public void IsCorrectGroup_NotValid_False()
        {
            var result = Narfu.StudentsSchedule.IsCorrectGroup(3);
            Assert.False(result);
        }
    }
}
