namespace Goblin.Application.Core.Abstractions;

public interface ITextCommand
{
    bool IsAdminCommand { get; }

    string[] Aliases { get; }

    Task<CommandExecutionResult> Execute(Message msg, BotUser user);
}