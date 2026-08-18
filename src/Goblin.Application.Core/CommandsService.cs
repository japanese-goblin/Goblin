using Goblin.DataAccess;
using Goblin.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Goblin.Application.Core;

public class CommandsService(
    IEnumerable<ITextCommand> textCommands,
    IEnumerable<IKeyboardCommand> keyboardCommands,
    IEnumerable<IUserFlow> userFlows,
    BotDbContext dbContext,
    ILogger<CommandsService> logger)
{
    private const string CommandNotFoundMessage = "Команда не найдена. Проверьте правильность написания команды. " +
                                                  "Если вы хотите отключить подобные ошибки, то, пожалуйста, напишите команду 'мут'";

    public async Task<CommandExecutionResult> ExecuteAction(Message msg, CancellationToken ct)
    {
        var user = await GetBotUserV2(msg.ConsumerType, msg.UserId, ct);
        var userFlow = userFlows.FirstOrDefault(p => p.Type == user.Session.FlowType);
        if (userFlow is null)
        {
            return CommandExecutionResult.Failed(CommandNotFoundMessage);
        }

        var context = new UserFlowContext(user, msg);
        var executionResult = await userFlow.HandleAsync(context, ct);

        user.Session.FlowType = executionResult.FlowType;
        user.Session.FlowStepType = executionResult.FlowState;
        // TODO: session data?
        await dbContext.SaveChangesAsync(ct);

        return CommandExecutionResult.Success(executionResult.Message, executionResult.Keyboard);
    }

    public async Task ExecuteCommand(Message msg,
        Func<CommandExecutionResult, Task> onSuccess,
        Func<CommandExecutionResult, Task> onFailed)
    {
        CommandExecutionResult result;
        var user = await GetBotUser(msg.UserId, msg.ConsumerType);
        if (!string.IsNullOrWhiteSpace(msg.Payload))
        {
            result = await ExecuteKeyboardCommand(msg, user);
        }
        else
        {
            result = await ExecuteTextCommand(msg, user);
        }

        result.Keyboard ??= DefaultKeyboards.GetDefaultKeyboard();

        if (!result.IsSuccessful)
        {
            // если команда не найдена, и у пользователя отключены ошибки
            if (result is { IsSuccessful: false } && !user.IsErrorsEnabled)
            {
                return;
            }

            result.Message = $"❌ Ошибка: {result.Message}";
            await onFailed(result);
        }
        else
        {
            await onSuccess(result);
        }
    }

    private async Task<CommandExecutionResult> ExecuteTextCommand(Message msg, BotUser user)
    {
        logger.LogDebug("Обработка текстовой команды");
        var cmdName = msg.CommandName;

        foreach (var command in textCommands)
        {
            if (!command.Aliases.Contains(cmdName))
            {
                continue;
            }

            if (command.IsAdminCommand && !user.IsAdmin)
            {
                continue;
            }

            logger.LogDebug("Выполнение команды {CommandType}", command.GetType());
            var result = await command.Execute(msg, user);
            logger.LogDebug("Команда вернула результат: {IsExecutionSuccess}", result.IsSuccessful);

            return result;
        }

        return CommandExecutionResult.Failed(CommandNotFoundMessage);
    }

    private async Task<CommandExecutionResult> ExecuteKeyboardCommand(Message msg, BotUser user)
    {
        logger.LogDebug("Обработка команды с клавиатуры");
        var record = msg.ParsedPayload.First();
        foreach (var command in keyboardCommands)
        {
            if (!record.Key.Contains(command.Trigger))
            {
                continue;
            }

            logger.LogDebug("Выполнение команды с клавиатуры {CommandType}", command.GetType());
            return await command.Execute(msg, user);
        }

        return CommandExecutionResult.Failed(CommandNotFoundMessage);
    }

    private async Task<BotUser> GetBotUser(long userId, ConsumerType type)
    {
        var user = await dbContext.BotUsers
            .FirstOrDefaultAsync(p => p.ConsumerType == type && p.ConsumerId == userId);
        if (user is not null)
        {
            return user;
        }

        user = new BotUser(userId)
        {
            ConsumerType = type
        };
        await dbContext.BotUsers.AddAsync(user);
        await dbContext.SaveChangesAsync();

        return user;
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