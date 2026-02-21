namespace Goblin.Application.Core.Commands.Merged;

public class StartCommand : IKeyboardCommand, ITextCommand
{
    public bool IsAdminCommand => false;
    public string[] Aliases => ["старт", "начать", "/start"];

    public string Trigger => "command";

    public Task<CommandExecutionResult> Execute(Message msg, BotUser user)
    {
        return Task.FromResult(CommandExecutionResult.Success("Выберите действие:", DefaultKeyboards.GetDefaultKeyboard()));
    }
}