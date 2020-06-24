using System;
using System.Threading.Tasks;
using FluentAssertions;
using Goblin.Application.Core.Abstractions;
using Goblin.Application.Core.Commands.Keyboard;
using Goblin.Application.Core.Results.Failed;
using Goblin.Application.Core.Results.Success;
using Goblin.Domain.Entities;
using Moq;
using Xunit;

namespace Goblin.Application.Core.Tests.Commands.Keyboard
{
    public class WeatherDailyCommandTests : TestBase
    {
        private IWeatherService GetWeatherService()
        {
            var mock = new Mock<IWeatherService>();
            mock.Setup(x => x.GetDailyWeather(It.IsAny<string>(), It.IsAny<DateTime>()))
                .ReturnsAsync(new SuccessfulResult
                {
                    Message = "weather"
                });

            return mock.Object;
        }

        [Fact]
        public async Task ShouldReturnSuccessfulResult()
        {
            var command = new WeatherDailyCommand(GetWeatherService());
            var message = GenerateMessageWithPayload(DefaultUser.Id, DefaultUser.Id, command.Trigger, "11.11.2011");

            var result = await command.Execute<VkBotUser>(message, DefaultUser);
            result.Should().BeOfType<SuccessfulResult>();
            result.Message.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task ShouldReturnFailedResult_Because_DictionaryKeyDoesNotExists()
        {
            var command = new WeatherDailyCommand(GetWeatherService());
            var message = GenerateMessageWithPayload(DefaultUser.Id, DefaultUser.Id, "asd", "11.11.2011");

            var result = await command.Execute<VkBotUser>(message, DefaultUser);
            result.Should().BeOfType<FailedResult>();
            result.Message.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task ShouldReturnFailedResult_Because_DictionaryValueIsNotCorrectDate()
        {
            var command = new WeatherDailyCommand(GetWeatherService());
            var message = GenerateMessageWithPayload(DefaultUser.Id, DefaultUser.Id, command.Trigger, "20.20.2020");

            var result = await command.Execute<VkBotUser>(message, DefaultUser);
            result.Should().BeOfType<FailedResult>();
            result.Message.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task ShouldReturnFailedResult_Because_UserCityIsEmpty()
        {
            DefaultUser.SetCity(string.Empty);
            var command = new WeatherDailyCommand(GetWeatherService());
            var message = GenerateMessageWithPayload(DefaultUser.Id, DefaultUser.Id, command.Trigger, "11.11.2011");

            var result = await command.Execute<VkBotUser>(message, DefaultUser);
            result.Should().BeOfType<FailedResult>();
            result.Message.Should().NotBeNullOrEmpty();
        }
    }
}