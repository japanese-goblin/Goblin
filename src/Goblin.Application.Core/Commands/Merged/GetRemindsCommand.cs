using System.Text;
using Goblin.DataAccess;

namespace Goblin.Application.Core.Commands.Merged;

public class GetRemindsCommand : IKeyboardCommand, ITextCommand
{
    public string Trigger => "reminds";

    public bool IsAdminCommand => false;

    public string[] Aliases => ["напоминания"];

    private readonly BotDbContext _context;

    public GetRemindsCommand(BotDbContext context)
    {
        _context = context;
    }

    public Task<CommandExecutionResult> Execute(Message msg, BotUser user)
    {
        var reminds = _context.Reminds.Where(x => x.ChatId == user.Id && x.ConsumerType == user.ConsumerType)
                              .ToArray();
        if(reminds.Length == 0)
        {
            return Task.FromResult(CommandExecutionResult.Success("У Вас нет ни одного добавленного напоминания."));
        }

        var strBuilder = new StringBuilder();
        strBuilder.AppendLine("Список напоминаний:");
        foreach(var userRemind in reminds.OrderBy(x => x.Date))
        {
            strBuilder.Append($"{userRemind.Date:dd.MM.yyyy HH:mm} - {userRemind.Text}")
                      .AppendLine();
        }

        return Task.FromResult(CommandExecutionResult.Success(strBuilder.ToString()));
    }
}