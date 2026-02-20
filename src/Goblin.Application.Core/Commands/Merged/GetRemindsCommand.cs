using System.Text;
using Goblin.Application.Core.Abstractions;
using Goblin.Application.Core.Models;
using Goblin.Application.Core.Results.Success;
using Goblin.DataAccess;
using Goblin.Domain.Entities;

namespace Goblin.Application.Core.Commands.Merged;

public class GetRemindsCommand : IKeyboardCommand, ITextCommand
{
    public string Trigger => "reminds";

    public bool IsAdminCommand => false;
    public string[] Aliases => new[] { "напоминания" };

    private readonly BotDbContext _context;

    public GetRemindsCommand(BotDbContext context)
    {
        _context = context;
    }

    public Task<IResult> Execute(Message msg, BotUser user)
    {
        var reminds = _context.Reminds.Where(x => x.ChatId == user.Id && x.ConsumerType == user.ConsumerType)
                              .ToArray();
        if(reminds.Length == 0)
        {
            return Task.FromResult<IResult>(new SuccessfulResult
            {
                Message = "У Вас нет ни одного добавленного напоминания."
            });
        }

        var strBuilder = new StringBuilder();
        strBuilder.AppendLine("Список напоминаний:");
        foreach(var userRemind in reminds.OrderBy(x => x.Date))
        {
            strBuilder.AppendFormat("{0:dd.MM.yyyy HH:mm} - {1}", userRemind.Date, userRemind.Text)
                      .AppendLine();
        }

        return Task.FromResult<IResult>(new SuccessfulResult
        {
            Message = strBuilder.ToString()
        });
    }
}