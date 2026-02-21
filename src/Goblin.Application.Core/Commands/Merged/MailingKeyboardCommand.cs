namespace Goblin.Application.Core.Commands.Merged;

public class MailingKeyboardCommand : IKeyboardCommand, ITextCommand
{
    public string Trigger => "mailingKeyboard";

    public bool IsAdminCommand => false;

    public string[] Aliases => ["рассылка"];

    public Task<CommandExecutionResult> Execute(Message msg, BotUser user)
    {
        return Task.FromResult(CommandExecutionResult.Success("Выберите действие:", DefaultKeyboards.GetMailingKeyboard(user)));
    }
}