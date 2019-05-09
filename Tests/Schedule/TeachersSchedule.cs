using System.Threading.Tasks;
using Xunit;

namespace Tests.Schedule
{
    public class TeachersSchedule
    {
        private const int Id = 22913;

        [Fact]
        public async Task GetScheule_Correct()
        {
            var (isError, _) = await Narfu.TeachersSchedule.GetSchedule(Id);
            Assert.False(isError);
        }

        [Fact]
        public async Task GetScheduleToSend_Correct()
        {
            var result = await Narfu.TeachersSchedule.GetScheduleToSend(Id);
            Assert.True(result.Contains("преподавателя"));
        }

        [Fact]
        public async Task GetScheduleToSend_NotCorrect()
        {
            var result = await Narfu.TeachersSchedule.GetScheduleToSend(6);
            Assert.True(result.Contains("Ошибка"));
        }

        [Fact]
        public void FindByName_Correct()
        {
            var result = Narfu.TeachersSchedule.FindByName("деменков");
            Assert.False(string.IsNullOrEmpty(result));
        }

        [Fact]
        public void FindByName_NotCorrect()
        {
            var result = Narfu.TeachersSchedule.FindByName("апььрл");
            Assert.True(string.IsNullOrEmpty(result));
        }
    }
}