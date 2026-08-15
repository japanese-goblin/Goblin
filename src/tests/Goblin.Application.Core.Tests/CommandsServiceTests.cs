using FluentAssertions;
using Goblin.Application.Core.Abstractions;
using Goblin.Application.Core.Commands.Merged;
using Goblin.Application.Core.Tests.Models;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace Goblin.Application.Core.Tests;

public class CommandsServiceTests : TestBase
{
    private CommandsService GetService()
    {
        var service = new CommandsService(
            GetTextCommands(),
            GetKeyboardCommands(),
            [],
            GetDbContext(),
            Substitute.For<ILogger<CommandsService>>());

        return service;

        IEnumerable<IKeyboardCommand> GetKeyboardCommands() =>
            [new MailingKeyboardCommand(), new ScheduleKeyboardCommand()];

        IEnumerable<ITextCommand> GetTextCommands() => [new HelpCommand(), new StartCommand(), new FakeAdminCommand()];
    }

    private static Task OnSuccess(CommandExecutionResult res)
    {
        res.IsSuccessful.Should().BeTrue();
        return Task.CompletedTask;
    }

    private static Task OnFailed(CommandExecutionResult res)
    {
        res.IsSuccessful.Should().BeFalse();
        return Task.CompletedTask;
    }

    [Fact]
    public async Task ShouldExecuteOnSuccess_On_Text()
    {
        var service = GetService();
        var message = GenerateMessage(DefaultUser.ConsumerId, DefaultUser.ConsumerId, "справка");

        await service.ExecuteCommand(message, OnSuccess, _ => Task.CompletedTask);
    }

    [Fact]
    public async Task ShouldExecuteOnFailed_On_Text()
    {
        var service = GetService();
        var message = GenerateMessage(DefaultUser.ConsumerId, DefaultUser.ConsumerId, "абв");

        await service.ExecuteCommand(message, _ => Task.CompletedTask, OnFailed);
    }

    [Fact]
    public async Task ShouldExecuteOnFailed_Because_UserIsNotAdmin_On_Text()
    {
        var service = GetService();
        var message = GenerateMessage(DefaultUser.ConsumerId, DefaultUser.ConsumerId, "demo");

        await service.ExecuteCommand(message, _ => Task.CompletedTask, OnFailed);
    }

    [Fact]
    public async Task ShouldNotExecuteAnything_Because_CommandNotFound_And_UserErrorsIsDisabled_On_Text()
    {
        DefaultUser.SetErrorNotification(false);
        var service = GetService();
        var message = GenerateMessage(DefaultUser.ConsumerId, DefaultUser.ConsumerId, "абв");

        await service.ExecuteCommand(message, _ => Task.CompletedTask, _ => Task.CompletedTask);
    }

    [Fact]
    public async Task ShouldExecuteOnSuccess_On_Payload()
    {
        var service = GetService();
        var message = GenerateMessageWithPayload(DefaultUser.ConsumerId, DefaultUser.ConsumerId, "mailingKeyboard",
            string.Empty);

        await service.ExecuteCommand(message, OnSuccess, _ => Task.CompletedTask);
    }

    [Fact]
    public async Task ShouldExecuteOnFailed_Because_CommandNotFound_On_Payload()
    {
        var service = GetService();
        var message = GenerateMessageWithPayload(DefaultUser.ConsumerId, DefaultUser.ConsumerId, "asd", string.Empty);

        await service.ExecuteCommand(message, _ => Task.CompletedTask, OnFailed);
    }
}