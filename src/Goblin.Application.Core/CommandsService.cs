using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Goblin.Application.Core.Abstractions;
using Goblin.Application.Core.Models;
using Goblin.Application.Core.Results.Failed;
using Goblin.DataAccess;
using Goblin.Domain;
using Goblin.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Goblin.Application.Core;

public class CommandsService
{
    private readonly BotDbContext _context;
    private readonly IEnumerable<IKeyboardCommand> _keyboardCommands;
    private readonly ILogger _logger;
    private readonly IEnumerable<ITextCommand> _textCommands;

    public CommandsService(IEnumerable<ITextCommand> textCommands, IEnumerable<IKeyboardCommand> keyboardCommands,
                           BotDbContext context, ILogger<CommandsService> logger)
    {
        _textCommands = textCommands;
        _keyboardCommands = keyboardCommands;
        _context = context;
        _logger = logger;
    }

    public async Task ExecuteCommand(Message msg,
                                        Func<IResult, Task> onSuccess,
                                        Func<IResult, Task> onFailed)
    {
        IResult result;
        var user = await GetBotUser(msg.UserId, msg.ConsumerType);
        if(!string.IsNullOrWhiteSpace(msg.Payload))
        {
            result = await ExecuteKeyboardCommand(msg, user);
        }
        else
        {
            result = await ExecuteTextCommand(msg, user);
        }

        result.Keyboard ??= DefaultKeyboards.GetDefaultKeyboard();

        if(!result.IsSuccessful)
        {
            if(result is CommandNotFoundResult && !user.IsErrorsEnabled)
            {
                // если команда не найдена, и у пользователя отключены ошибки
                return;
            }

            await onFailed(result);
        }
        else
        {
            await onSuccess(result);
        }
    }

    private async Task<IResult> ExecuteTextCommand(Message msg, BotUser user)
    {
        _logger.LogDebug("Обработка текстовой команды");
        var cmdName = msg.CommandName;

        foreach(var command in _textCommands)
        {
            if(!command.Aliases.Contains(cmdName))
            {
                continue;
            }

            if(command.IsAdminCommand && !user.IsAdmin)
            {
                continue;
            }

            _logger.LogDebug("Выполнение команды {CommandType}", command.GetType());
            var result = await command.Execute(msg, user);
            _logger.LogDebug("Команда вернула {ResultType} результат", result.GetType());

            return result;
        }

        return new CommandNotFoundResult();
    }

    private async Task<IResult> ExecuteKeyboardCommand(Message msg, BotUser user)
    {
        _logger.LogDebug("Обработка команды с клавиатуры");
        var record = msg.ParsedPayload.First();
        foreach(var command in _keyboardCommands)
        {
            if(!record.Key.Contains(command.Trigger))
            {
                continue;
            }

            _logger.LogDebug("Выполнение команды с клавиатуры {CommandType}", command.GetType());
            return await command.Execute(msg, user);
        }

        return new CommandNotFoundResult();
    }

    private async Task<BotUser> GetBotUser(long userId, ConsumerType type)
    {
        var user = await _context.BotUsers.FindAsync(userId, type);
        if(user is not null)
        {
            return user;
        }

        user = new BotUser(userId)
        {
            ConsumerType = type
        };
        await _context.BotUsers.AddAsync(user);
        await _context.SaveChangesAsync();

        return user;
    }
}