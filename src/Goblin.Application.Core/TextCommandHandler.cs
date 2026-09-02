namespace Goblin.Application.Core;

public class TextCommandHandler(IEnumerable<ITextCommand> commands)
{
    private const string CommandNotFoundMessage = "Команда не найдена. Проверьте правильность написания команды.";

    public async Task<CommandExecutionResult?> TryHandle(Message message, BotUser user)
    {
        if (!string.IsNullOrWhiteSpace(message.Payload) || message.CommandName is not { } commandName)
        {
            return null;
        }

        var command = commands.FirstOrDefault(p => p.Aliases.Contains(commandName));
        if (command is null)
        {
            return null;
        }

        if (command.IsAdminCommand && !user.IsAdmin)
        {
            return CommandExecutionResult.Failed(CommandNotFoundMessage);
        }

        return await command.Execute(message, user);
    }
}
