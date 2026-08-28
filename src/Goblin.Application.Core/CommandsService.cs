using Goblin.DataAccess;
using Goblin.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Goblin.Application.Core;

public class CommandsService(
    TextCommandHandler textCommandHandler,
    IEnumerable<IUserFlow> userFlows,
    BotDbContext dbContext)
{
    private const string CommandNotFoundMessage = "Команда не найдена. Проверьте правильность написания команды.";

    public async Task<CommandExecutionResult> ExecuteAction(Message msg, CancellationToken ct)
    {
        var user = await GetBotUserV2(msg.ConsumerType, msg.UserId, ct);
        var commandResult = await textCommandHandler.TryHandle(msg, user);
        if(commandResult is not null)
        {
            await dbContext.SaveChangesAsync(ct);
            return commandResult;
        }

        var parsedPayload = msg.ParsedPayload;
        var userFlow = parsedPayload is null
            ? null
            : userFlows.FirstOrDefault(flow => parsedPayload.ContainsKey(flow.PayloadKey));

        userFlow ??= userFlows.FirstOrDefault(flow => flow.Type == user.Session.FlowType);
        if (userFlow is null)
        {
            return CommandExecutionResult.Failed(CommandNotFoundMessage);
        }

        var context = new UserFlowContext(user, msg);
        var executionResult = await userFlow.HandleAsync(context, ct);

        user.Session.FlowType = executionResult.FlowType;
        user.Session.FlowStepType = executionResult.FlowState;
        await dbContext.SaveChangesAsync(ct);

        return executionResult.IsSuccessful
            ? CommandExecutionResult.Success(executionResult.Message, executionResult.Keyboard)
            : CommandExecutionResult.Failed(executionResult.Message, executionResult.Keyboard);
    }

    private async Task<BotUser> GetBotUserV2(ConsumerType type, long userId, CancellationToken ct)
    {
        var user = await dbContext.BotUsers
            .Include(p => p.Session)
            .FirstOrDefaultAsync(p => p.ConsumerType == type && p.ConsumerId == userId, ct);
        if (user is not null)
        {
            return user;
        }

        user = new BotUser(userId)
        {
            ConsumerType = type,
            Session = new BotUserSession
            {
                FlowType = FlowType.Start
            }
        };
        await dbContext.BotUsers.AddAsync(user, ct);
        await dbContext.SaveChangesAsync(ct);

        return user;
    }
}
