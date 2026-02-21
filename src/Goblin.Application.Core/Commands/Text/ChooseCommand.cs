namespace Goblin.Application.Core.Commands.Text;

public class ChooseCommand : ITextCommand
{
    public bool IsAdminCommand => false;

    public string[] Aliases => ["выбери", "рандом"];

    public Task<CommandExecutionResult> Execute(Message msg, BotUser user)
    {
        var param = string.Join(' ', msg.CommandParameters);
        var split = Split(param);

        if(split.Length < 2)
        {
            const string text = "Введите два или более предложений, разделенных следующими символами: ',' и 'или'";
            return Task.FromResult(CommandExecutionResult.Failed(text));
        }

        var random = GetRandom(0, split.Length);

        return Task.FromResult(CommandExecutionResult.Success($"Я выбираю это: {split[random]}"));
    }

    private static int GetRandom(int start, int end)
    {
        return new Random(DateTime.Now.Millisecond).Next(start, end);
    }

    private static string[] Split(string str)
    {
        return str.Split([",", ", ", " или "], StringSplitOptions.RemoveEmptyEntries);
    }
}