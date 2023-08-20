using FluentAssertions;
using Xunit;

namespace Goblin.Narfu.Tests.StudentsSchedule;

public class IsCorrectGroupTests : TestBase
{
    [Fact(Skip = "Доработать с NSubstitute")]
    public void IsCorrectGroup_CorrectGroup_ReturnsTrue()
    {
        var result = Api.Students.IsCorrectGroup(CorrectGroup);

        result.Should().BeTrue();
    }

    [Fact(Skip = "Доработать с NSubstitute")]
    public void IsCorrectGroup_IncorrectGroup_ReturnsTrue()
    {
        var result = Api.Students.IsCorrectGroup(IncorrectGroup);

        result.Should().BeFalse();
    }
}