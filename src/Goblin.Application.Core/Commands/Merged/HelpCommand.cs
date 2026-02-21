namespace Goblin.Application.Core.Commands.Merged;

public class HelpCommand : IKeyboardCommand, ITextCommand
{
    public bool IsAdminCommand => false;

    public string[] Aliases => ["помоги", "справка", "помощь", "команды"];

    public string Trigger => "help";

    public Task<CommandExecutionResult> Execute(Message msg, BotUser user)
    {
        // TODO: move to settings
        const string guideLink = "https://vk.com/@-146048760-commands";
        return Task.FromResult(CommandExecutionResult.Success($"Список всех доступных команд здесь: {guideLink}"));
    }
}