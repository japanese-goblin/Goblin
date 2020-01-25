using Xunit;

namespace Goblin.Narfu.Tests.StudentsSchedule
{
    public class IsCorrectGroupTests : TestBase
    {
        [Fact]
        public void IsCorrectGroup_CorrectGroup_ReturnsTrue()
        {
            var correct = Api.Students.IsCorrectGroup(CorrectGroup);

            Assert.True(correct);
        }

        [Fact]
        public void IsCorrectGroup_IncorrectGroup_ReturnsTrue()
        {
            var incorrect = Api.Students.IsCorrectGroup(IncorrectGroup);

            Assert.False(incorrect);
        }
    }
}