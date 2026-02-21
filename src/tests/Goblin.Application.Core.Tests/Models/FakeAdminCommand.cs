using Goblin.Application.Core.Abstractions;
using Goblin.Application.Core.Models;
using Goblin.Domain.Entities;

namespace Goblin.Application.Core.Tests.Models;

public class FakeAdminCommand : ITextCommand
{
    public bool IsAdminCommand => true;

    public string[] Aliases => ["demo"];

    public Task<CommandExecutionResult> Execute(Message msg, BotUser user)
    {
        return Task.FromResult(CommandExecutionResult.Success("demo"));
    }
}