using System.Threading.Tasks;
using FluentAssertions;
using Goblin.Application.Core.Commands.Text;
using Goblin.Application.Core.Results.Failed;
using Goblin.Application.Core.Results.Success;
using Goblin.Narfu.Abstractions;
using Goblin.Narfu.Models;
using Goblin.OpenWeatherMap.Abstractions;
using Moq;
using Xunit;

namespace Goblin.Application.Core.Tests.Commands.Text;

public class SetDataCommandTests : TestBase
{
    [Theory]
    [InlineData("город Москва")]
    [InlineData("группу 353535")]
    [InlineData("группа 353535")]
    public async Task ShouldReturnSuccessfulResult(string parameters)
    {
        var command = new SetDataCommand(ApplicationContext, GetWeatherApi(), GetNarfuApi());
        var text = $"{command.Aliases[0]} {parameters}";
        var message = GenerateMessage(DefaultUser.Id, DefaultUser.Id, text);

        var result = await command.Execute(message, DefaultUser);

        result.Should().BeOfType<SuccessfulResult>();
        result.Message.Should().NotBeNullOrEmpty();
    }

    private INarfuApi GetNarfuApi(bool response = true)
    {
        var mockApi = new Mock<INarfuApi>();
        mockApi.Setup(x => x.Students.IsCorrectGroup(It.IsAny<int>()))
               .Returns(response);
        mockApi.Setup(x => x.Students.GetGroupByRealId(It.IsAny<int>()))
               .Returns(new Group
               {
                   Name = "name",
                   RealId = 1,
                   SiteId = 1
               });

        return mockApi.Object;
    }

    private IOpenWeatherMapApi GetWeatherApi(bool response = true)
    {
        var mockWeather = new Mock<IOpenWeatherMapApi>();
        mockWeather.Setup(x => x.IsCityExists(It.IsAny<string>()))
                   .ReturnsAsync(response);
        return mockWeather.Object;
    }

    [Fact]
    public async Task ShouldReturnFailedResult_Because_CityIsNotExists()
    {
        var command = new SetDataCommand(ApplicationContext, GetWeatherApi(false), GetNarfuApi());
        var text = $"{command.Aliases[0]} город абв";
        var message = GenerateMessage(DefaultUser.Id, DefaultUser.Id, text);

        var result = await command.Execute(message, DefaultUser);

        result.Should().BeOfType<FailedResult>();
        result.Message.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task ShouldReturnFailedResult_Because_GroupIdIsNotInteger()
    {
        var command = new SetDataCommand(ApplicationContext, GetWeatherApi(), GetNarfuApi());
        var text = $"{command.Aliases[0]} группу абв";
        var message = GenerateMessage(DefaultUser.Id, DefaultUser.Id, text);

        var result = await command.Execute(message, DefaultUser);

        result.Should().BeOfType<FailedResult>();
        result.Message.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task ShouldReturnFailedResult_Because_GroupIsNotExists()
    {
        var command = new SetDataCommand(ApplicationContext, GetWeatherApi(), GetNarfuApi(false));
        var text = $"{command.Aliases[0]} группу 353535";
        var message = GenerateMessage(DefaultUser.Id, DefaultUser.Id, text);

        var result = await command.Execute(message, DefaultUser);

        result.Should().BeOfType<FailedResult>();
        result.Message.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task ShouldReturnFailedResult_Because_IncorrectSetParameter()
    {
        var command = new SetDataCommand(ApplicationContext, GetWeatherApi(false), GetNarfuApi());
        var text = $"{command.Aliases[0]} абв абв";
        var message = GenerateMessage(DefaultUser.Id, DefaultUser.Id, text);

        var result = await command.Execute(message, DefaultUser);

        result.Should().BeOfType<FailedResult>();
        result.Message.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task ShouldReturnFailedResult_Because_NotEnoughParameters()
    {
        var command = new SetDataCommand(ApplicationContext, GetWeatherApi(), GetNarfuApi());
        var text = $"{command.Aliases[0]} город";
        var message = GenerateMessage(DefaultUser.Id, DefaultUser.Id, text);

        var result = await command.Execute(message, DefaultUser);

        result.Should().BeOfType<FailedResult>();
        result.Message.Should().NotBeNullOrEmpty();
    }
}