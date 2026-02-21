using Goblin.DataAccess;
using Goblin.Domain;
using Microsoft.EntityFrameworkCore;

namespace Goblin.Application.Core.Commands.Text;

public class SendToAdminCommand(BotDbContext db, IEnumerable<ISender> senders) : ITextCommand
{
    public bool IsAdminCommand => false;
    public string[] Aliases => ["админ", "сообщение"];

    public async Task<CommandExecutionResult> Execute(Message msg, BotUser user)
    {
        var text = string.Join(' ', msg.CommandParameters);
        if(string.IsNullOrWhiteSpace(text))
        {
            return CommandExecutionResult.Failed("Введите текст сообщения.");
        }

        var message = $"Сообщение от {msg.UserTag}:\n{text}";
        var adminUsers = await db.BotUsers.Where(x => x.IsAdmin &&
                                                       x.ConsumerType == user.ConsumerType)
                                  .Select(x => x.Id)
                                  .ToArrayAsync();
        var sender = senders.First(x => x.ConsumerType == ConsumerType.Vkontakte);

        foreach(var admin in adminUsers)
        {
            await sender.Send(admin, message);
        }

        return CommandExecutionResult.Success("Ваше сообщение успешно отправлено администрации");
    }
}