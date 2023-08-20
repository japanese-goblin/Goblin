﻿using System.Threading.Tasks;
using FluentAssertions;
using Goblin.Application.Core.Commands.Text;
using Goblin.Application.Core.Results.Failed;
using Goblin.Application.Core.Results.Success;
using Goblin.Narfu.Abstractions;
using Goblin.Narfu.Models;
using Goblin.OpenWeatherMap.Abstractions;
using NSubstitute;
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

    private static INarfuApi GetNarfuApi(bool response = true)
    {
        var mockApi = Substitute.For<INarfuApi>();
        mockApi.Students.IsCorrectGroup(Arg.Any<int>())
               .Returns(response);
        mockApi.Students.GetGroupByRealId(Arg.Any<int>())
               .Returns(new Group
               {
                   Name = "name",
                   RealId = 1,
                   SiteId = 1
               });
        return mockApi;
    }

    private static IOpenWeatherMapApi GetWeatherApi(bool response = true)
    {
        var mockWeather = Substitute.For<IOpenWeatherMapApi>();
        mockWeather.IsCityExists(Arg.Any<string>())
                   .Returns(response);
        return mockWeather;
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