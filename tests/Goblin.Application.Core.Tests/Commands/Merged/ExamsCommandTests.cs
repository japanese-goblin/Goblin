using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Flurl;
using Flurl.Http;
using Goblin.Application.Core.Commands.Merged;
using Goblin.Application.Core.Results.Failed;
using Goblin.Application.Core.Results.Success;
using Goblin.Narfu.Abstractions;
using Goblin.Narfu.Models;
using Goblin.Narfu.ViewModels;
using Moq;
using Xunit;

namespace Goblin.Application.Core.Tests.Commands.Merged
{
    public class ExamsCommandTests : TestBase
    {
        private INarfuApi GetNarfuApi()
        {
            var mockApi = new Mock<INarfuApi>();
            mockApi.Setup(x => x.Students.GetExams(It.IsAny<int>()))
                   .ReturnsAsync(new ExamsViewModel(new List<Lesson>(), DateTime.Today));

            return mockApi.Object;
        }

        private INarfuApi GetNarfuApiWithFlurlException()
        {
            const string endPoint = "https://localhost";
            var mockApi = new Mock<INarfuApi>();
            mockApi.Setup(x => x.Students.GetExams(It.IsAny<int>()))
                   .ThrowsAsync(new FlurlHttpException(new FlurlCall()
                   {
                       Request = new FlurlRequest(new Url(endPoint)),
                       HttpRequestMessage = new HttpRequestMessage(HttpMethod.Get, endPoint)
                   }));

            return mockApi.Object;
        }

        private INarfuApi GetNarfuApiWithException()
        {
            var mockApi = new Mock<INarfuApi>();
            mockApi.Setup(x => x.Students.GetExams(It.IsAny<int>()))
                   .ThrowsAsync(new Exception());

            return mockApi.Object;
        }

        [Fact]
        public async Task ShouldReturnSuccessfulResult()
        {
            var command = new ExamsCommand(GetNarfuApi());
            var message = GenerateMessage(DefaultUser.Id, DefaultUser.Id, command.Aliases[0]);

            var result = await command.Execute(message, DefaultUser);
            result.Should().BeOfType<SuccessfulResult>();
            result.Message.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task ShouldReturnFailedResult_Because_UserGroupIsZero()
        {
            DefaultUser.SetNarfuGroup(0);
            var command = new ExamsCommand(GetNarfuApi());
            var message = GenerateMessage(DefaultUser.Id, DefaultUser.Id, command.Aliases[0]);

            var result = await command.Execute(message, DefaultUser);
            result.Should().BeOfType<FailedResult>();
            result.Message.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task ShouldReturnFailedResult_Because_SiteIsUnavailable()
        {
            var command = new ExamsCommand(GetNarfuApiWithFlurlException());
            var message = GenerateMessage(DefaultUser.Id, DefaultUser.Id, command.Aliases[0]);

            var result = await command.Execute(message, DefaultUser);
            result.Should().BeOfType<FailedResult>();
            result.Message.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task ShouldReturnFailedResult_Because_UnknownError()
        {
            var command = new ExamsCommand(GetNarfuApiWithException());
            var message = GenerateMessage(DefaultUser.Id, DefaultUser.Id, command.Aliases[0]);

            var result = await command.Execute(message, DefaultUser);
            result.Should().BeOfType<FailedResult>();
            result.Message.Should().NotBeNullOrEmpty();
        }
    }
}