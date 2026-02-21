using Goblin.DataAccess;

namespace Goblin.Application.Core.Commands.Text;

public class MuteCommand : ITextCommand
{
    public bool IsAdminCommand => false;
    public string[] Aliases => ["мут"];
    private readonly BotDbContext _db;

    public MuteCommand(BotDbContext db)
    {
        _db = db;
    }

    public async Task<CommandExecutionResult> Execute(Message msg, BotUser user)
    {
        user = _db.Entry(user).Entity;
        user.SetErrorNotification(!user.IsErrorsEnabled);
        await _db.SaveChangesAsync();
        return CommandExecutionResult.Success(user.IsErrorsEnabled ? "Ошибки включены" : "Ошибки отключены");
    }
}