namespace Goblin.Application.Core.Commands.Text;

public class RemoveKeyboardCommand : ITextCommand
{
    public bool IsAdminCommand => false;
    public string[] Aliases => ["куб"];

    public Task<CommandExecutionResult> Execute(Message msg, BotUser user)
    {
        var kb = new CoreKeyboard();
        return Task.FromResult(CommandExecutionResult.Success("Окей", kb));
    }
}