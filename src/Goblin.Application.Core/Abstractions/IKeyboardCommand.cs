namespace Goblin.Application.Core.Abstractions;

public interface IKeyboardCommand
{
    string Trigger { get; }

    Task<CommandExecutionResult> Execute(Message msg, BotUser user);
}