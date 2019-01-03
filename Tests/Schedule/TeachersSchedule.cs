using System.Threading.Tasks;
using Xunit;

namespace Tests.Schedule
{
    public class TeachersSchedule
    {
        private const int CorrectId = 22913;

        [Fact]
        public async Task GetScheule_Correct()
        {
            var (isError, _) = await Goblin.Schedule.TeachersSchedule.GetScheule(CorrectId);
            Assert.False(isError);
        }

        [Fact]
        public async Task GetScheduleToSend_Correct()
        {
            var result = await Goblin.Schedule.TeachersSchedule.GetScheduleToSend(CorrectId);
            Assert.True(result.Contains("преподавателя"));
        }

        [Fact]
        public async Task GetScheduleToSend_NotCorrect()
        {
            var result = await Goblin.Schedule.TeachersSchedule.GetScheduleToSend(6);
            Assert.True(result.Contains("Ошибка"));
        }

        [Fact]
        public void FindByName_Correct()
        {
            var result = Goblin.Schedule.TeachersSchedule.FindByName("деменков");
            Assert.False(string.IsNullOrEmpty(result));
        }

        [Fact]
        public void FindByName_NotCorrect()
        {
            var result = Goblin.Schedule.TeachersSchedule.FindByName("апььрл");
            Assert.True(string.IsNullOrEmpty(result));
        }
    }
}
