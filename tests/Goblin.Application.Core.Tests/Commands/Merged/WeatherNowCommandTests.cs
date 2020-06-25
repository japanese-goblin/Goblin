using System.Threading.Tasks;
using FluentAssertions;
using Goblin.Application.Core.Abstractions;
using Goblin.Application.Core.Commands.Merged;
using Goblin.Application.Core.Results.Failed;
using Goblin.Application.Core.Results.Success;
using Goblin.Domain.Entities;
using Moq;
using Xunit;

namespace Goblin.Application.Core.Tests.Commands.Merged
{
    public class WeatherNowCommandTests : TestBase
    {
        private IWeatherService GetWeatherService()
        {
            var mock = new Mock<IWeatherService>();
            mock.Setup(x => x.GetCurrentWeather(It.IsAny<string>()))
                .ReturnsAsync(new SuccessfulResult
                {
                    Message = "weather"
                });

            return mock.Object;
        }

        [Fact]
        public async Task ShouldReturnSuccessfulResultWithText_Because_UserHasSetCity()
        {
            var command = new WeatherNowCommand(GetWeatherService());
            var message = GenerateMessage(DefaultUser.Id, DefaultUser.Id, command.Aliases[0]);

            var result = await command.Execute<VkBotUser>(message, DefaultUser);
            result.Should().BeOfType<SuccessfulResult>();
            result.Message.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task ShouldReturnFailedResult_Because_UserCityAndParameterAreEmpty()
        {
            DefaultUser.SetCity(string.Empty);
            var command = new WeatherNowCommand(GetWeatherService());
            var message = GenerateMessage(DefaultUser.Id, DefaultUser.Id, command.Aliases[0]);

            var result = await command.Execute<VkBotUser>(message, DefaultUser);
            result.Should().BeOfType<FailedResult>();
            result.Message.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task ShouldReturnSuccessfulResult_Because_ParameterIsNotEmpty()
        {
            var command = new WeatherNowCommand(GetWeatherService());
            var text = $"{command.Aliases[0]} Москва";
            var message = GenerateMessage(DefaultUser.Id, DefaultUser.Id, text);

            var result = await command.Execute<VkBotUser>(message, DefaultUser);
            result.Should().BeOfType<SuccessfulResult>();
            result.Message.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task ShouldReturnSuccessfulResultWithPayload_Because_UserHasSetCity()
        {
            var command = new WeatherNowCommand(GetWeatherService());
            var message = GenerateMessageWithPayload(DefaultUser.Id, DefaultUser.Id, command.Trigger, string.Empty);

            var result = await command.Execute<VkBotUser>(message, DefaultUser);
            result.Should().BeOfType<SuccessfulResult>();
            result.Message.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task ShouldReturnFailedResultWithPayload_Because_UserDoesNotSetCity()
        {
            DefaultUser.SetCity(string.Empty);
            var command = new WeatherNowCommand(GetWeatherService());
            var message = GenerateMessageWithPayload(DefaultUser.Id, DefaultUser.Id, command.Trigger, string.Empty);

            var result = await command.Execute<VkBotUser>(message, DefaultUser);
            result.Should().BeOfType<FailedResult>();
            result.Message.Should().NotBeNullOrEmpty();
        }
    }
}