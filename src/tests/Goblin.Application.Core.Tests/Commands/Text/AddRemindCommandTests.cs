using FluentAssertions;
using Goblin.Application.Core.Commands.Text;
using Xunit;

namespace Goblin.Application.Core.Tests.Commands.Text;

public class AddRemindCommandTests : TestBase
{
    [Theory, InlineData("сегодня"), InlineData("завтра"), InlineData("21.01.2150")]
    public async Task ShouldReturnSuccessfulResult(string date)
    {
        var command = new AddRemindCommand(GetDbContext());
        var text = $"{command.Aliases[0]} {date} 23:59 тест";
        var message = GenerateMessage(DefaultUser.Id, DefaultUser.Id, text);

        var result = await command.Execute(message, DefaultUser);

        result.IsSuccessful.Should().BeTrue();
        result.Message.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task ShouldReturnFailedResult_Because_DateIsIncorrect()
    {
        var command = new AddRemindCommand(GetDbContext());
        var text = $"{command.Aliases[0]} 45.01.2010 23:59 тест";
        var message = GenerateMessage(DefaultUser.Id, DefaultUser.Id, text);

        var result = await command.Execute(message, DefaultUser);

        result.IsSuccessful.Should().BeFalse();
        result.Message.Should().Be("Некорректная дата или время");
    }

    [Fact]
    public async Task ShouldReturnFailedResult_Because_DateIsOlderThanNow()
    {
        var command = new AddRemindCommand(GetDbContext());
        var text = $"{command.Aliases[0]} 01.01.2010 23:59 тест";
        var message = GenerateMessage(DefaultUser.Id, DefaultUser.Id, text);

        var result = await command.Execute(message, DefaultUser);

        result.IsSuccessful.Should().BeFalse();
        result.Message.Should().Be("Дата напоминания меньше текущей");
    }

    [Fact]
    public async Task ShouldReturnFailedResult_Because_NotEnoughParameters()
    {
        var command = new AddRemindCommand(GetDbContext());
        var text = $"{command.Aliases[0]} сегодня 23:59";
        var message = GenerateMessage(DefaultUser.Id, DefaultUser.Id, text);

        var result = await command.Execute(message, DefaultUser);

        result.IsSuccessful.Should().BeFalse();
        result.Message.Should().Be("Укажите дату, время и текст напоминания (11.11.2011 11:11 текст)");
    }

    [Fact]
    public async Task TaskShouldReturnFailedResult_Because_MaxRemindsCount()
    {
        var command = new AddRemindCommand(GetDbContext());
        var text = $"{command.Aliases[0]} 01.01.2101 23:59 тест";
        var message = GenerateMessage(DefaultUserWithMaxReminds.Id, DefaultUserWithMaxReminds.Id, text);

        var result = await command.Execute(message, DefaultUserWithMaxReminds);

        result.IsSuccessful.Should().BeFalse();
        result.Message.Should().Be("Вы уже достигли максимального количества напоминаний (8)");
    }
}