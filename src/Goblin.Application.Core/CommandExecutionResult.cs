namespace Goblin.Application.Core;

public class CommandExecutionResult
{
    public bool IsSuccessful { get; init; }

    public required string Message { get; set; }

    public CoreKeyboard? Keyboard { get; set; }

    public static CommandExecutionResult Success(string message, CoreKeyboard? keyboard = null)
    {
        return new CommandExecutionResult
        {
            IsSuccessful = true,
            Message = message,
            Keyboard = keyboard
        };
    }

    public static CommandExecutionResult Failed(string message, CoreKeyboard? keyboard = null)
    {
        return new CommandExecutionResult
        {
            IsSuccessful = false,
            Message = message,
            Keyboard = keyboard
        };
    }
}