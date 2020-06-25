using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Goblin.Application.Core.Results.Failed;
using Goblin.Application.Core.Results.Success;
using Goblin.Application.Core.Services;
using Goblin.Narfu.Abstractions;
using Goblin.Narfu.Models;
using Goblin.Narfu.ViewModels;
using Moq;
using Xunit;

namespace Goblin.Application.Core.Tests.Services
{
    public class ScheduleServiceTests : TestBase
    {
        private INarfuApi GetNarfuApi(bool response = true)
        {
            var mock = new Mock<INarfuApi>();
            mock.Setup(x => x.Students.IsCorrectGroup(It.IsAny<int>()))
                .Returns(response);
            mock.Setup(x => x.Students.GetScheduleAtDate(It.IsAny<int>(), It.IsAny<DateTime>()))
                .ReturnsAsync(new LessonsViewModel(new List<Lesson>(), DateTime.Today));

            return mock.Object;
        }

        [Fact]
        public async Task ShouldReturnSuccessfulResult()
        {
            var service = new ScheduleService(GetNarfuApi());

            var result = await service.GetSchedule(DefaultUser.NarfuGroup, DateTime.Today);

            result.Should().BeOfType<SuccessfulResult>();
            result.Message.Should().NotBeNullOrWhiteSpace();
            result.Keyboard.Should().NotBeNull();
        }

        [Fact]
        public async Task ShouldReturnFailedResult_Because_UserGroupIsZero()
        {
            DefaultUser.SetNarfuGroup(0);
            var service = new ScheduleService(GetNarfuApi(false));

            var result = await service.GetSchedule(DefaultUser.NarfuGroup, DateTime.Today);

            result.Should().BeOfType<FailedResult>();
            result.Message.Should().NotBeNullOrWhiteSpace();
        }
    }
}