using System.Collections.Generic;
using System.Threading.Tasks;
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
        var service = new CommandsService(GetTextCommands(), GetKeyboardCommands(), ApplicationContext,
                                          Substitute.For<ILogger<CommandsService>>());

        return service;

        IEnumerable<IKeyboardCommand> GetKeyboardCommands()
        {
            return new IKeyboardCommand[] { new MailingKeyboardCommand(), new ScheduleKeyboardCommand() };
        }

        IEnumerable<ITextCommand> GetTextCommands()
        {
            return new ITextCommand[] { new HelpCommand(), new StartCommand(), new FakeAdminCommand() };
        }
    }

    private Task OnSuccess(IResult res)
    {
        res.IsSuccessful.Should().BeTrue();
        return Task.CompletedTask;
    }

    private Task OnFailed(IResult res)
    {
        res.IsSuccessful.Should().BeFalse();
        return Task.CompletedTask;
    }

    [Fact]
    public async Task ShouldExecuteOnSuccess_On_Text()
    {
        var service = GetService();
        var message = GenerateMessage(DefaultUser.Id, DefaultUser.Id, "справка");

        await service.ExecuteCommand(message, OnSuccess, res => null);
    }

    [Fact]
    public async Task ShouldExecuteOnFailed_On_Text()
    {
        var service = GetService();
        var message = GenerateMessage(DefaultUser.Id, DefaultUser.Id, "абв");

        await service.ExecuteCommand(message, res => null, OnFailed);
    }

    [Fact]
    public async Task ShouldExecuteOnFailed_Because_UserIsNotAdmin_On_Text()
    {
        var service = GetService();
        var message = GenerateMessage(DefaultUser.Id, DefaultUser.Id, "demo");

        await service.ExecuteCommand(message, res => null, OnFailed);
    }

    [Fact]
    public async Task ShouldNotExecuteAnything_Because_CommandNotFound_And_UserErrorsIsDisabled_On_Text()
    {
        DefaultUser.SetErrorNotification(false);
        var service = GetService();
        var message = GenerateMessage(DefaultUser.Id, DefaultUser.Id, "абв");

        await service.ExecuteCommand(message, res => null, res => null);
    }

    [Fact]
    public async Task ShouldExecuteOnSuccess_On_Payload()
    {
        var service = GetService();
        var message = GenerateMessageWithPayload(DefaultUser.Id, DefaultUser.Id, "mailingKeyboard", string.Empty);

        await service.ExecuteCommand(message, OnSuccess, res => null);
    }

    [Fact]
    public async Task ShouldExecuteOnFailed_Because_CommandNotFound_On_Payload()
    {
        var service = GetService();
        var message = GenerateMessageWithPayload(DefaultUser.Id, DefaultUser.Id, "asd", string.Empty);

        await service.ExecuteCommand(message, res => null, OnFailed);
    }
}