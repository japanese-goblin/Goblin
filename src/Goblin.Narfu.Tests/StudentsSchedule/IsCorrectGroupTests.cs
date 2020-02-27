﻿using FluentAssertions;
using Xunit;

namespace Goblin.Narfu.Tests.StudentsSchedule
{
    public class IsCorrectGroupTests : TestBase
    {
        [Fact]
        public void IsCorrectGroup_CorrectGroup_ReturnsTrue()
        {
            var result = Api.Students.IsCorrectGroup(CorrectGroup);

            result.Should().BeTrue();
        }

        [Fact]
        public void IsCorrectGroup_IncorrectGroup_ReturnsTrue()
        {
            var result = Api.Students.IsCorrectGroup(IncorrectGroup);

            result.Should().BeFalse();
        }
    }
}