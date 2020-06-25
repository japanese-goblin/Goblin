using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Goblin.Application.Core.Abstractions;
using Goblin.Application.Core.Results.Failed;
using Goblin.DataAccess;
using Goblin.Domain.Abstractions;
using Newtonsoft.Json;
using Serilog;

namespace Goblin.Application.Core
{
    public class CommandsService
    {
        private readonly BotDbContext _context;
        private readonly IEnumerable<IKeyboardCommand> _keyboardCommands;
        private readonly ILogger _logger;
        private readonly IEnumerable<ITextCommand> _textCommands;

        public CommandsService(IEnumerable<ITextCommand> textCommands,
                               IEnumerable<IKeyboardCommand> keyboardCommands,
                               BotDbContext context)
        {
            _textCommands = textCommands;
            _keyboardCommands = keyboardCommands;
            _context = context;
            _logger = Log.ForContext<CommandsService>();
        }

        public async Task ExecuteCommand<T>(IMessage msg,
                                            Func<IResult, Task> onSuccess,
                                            Func<IResult, Task> onFailed) where T : BotUser
        {
            IResult result;
            var user = await GetBotUser<T>(msg.MessageUserId);
            if(!string.IsNullOrWhiteSpace(msg.Payload))
            {
                result = await ExecuteKeyboardCommand<T>(msg, user);
            }
            else
            {
                result = await ExecuteTextCommand<T>(msg, user);
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

        private async Task<IResult> ExecuteTextCommand<T>(IMessage msg, BotUser user) where T : BotUser
        {
            _logger.Debug("Обработка текстовой команды");
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

                _logger.Debug("Выполнение команды {0}", command.GetType());
                var result = await command.Execute<T>(msg, user);
                _logger.Debug("Команда вернула {0} результат", result.GetType());

                return result;
            }

            return new CommandNotFoundResult();
        }

        private async Task<IResult> ExecuteKeyboardCommand<T>(IMessage msg, BotUser user) where T : BotUser
        {
            _logger.Debug("Обработка команды с клавиатуры");
            var record = JsonConvert.DeserializeObject<Dictionary<string, string>>(msg.Payload).FirstOrDefault();
            foreach(var command in _keyboardCommands)
            {
                if(!record.Key.Contains(command.Trigger))
                {
                    continue;
                }

                _logger.Debug("Выполнение команды с клавиатуры {0}", command.GetType());
                return await command.Execute<T>(msg, user);
            }

            return new CommandNotFoundResult();
        }

        private async Task<BotUser> GetBotUser<T>(long userId) where T : BotUser
        {
            var user = await _context.Set<T>()
                                     .FindAsync(userId);
            if(!(user is null))
            {
                return user;
            }

            var entity = Activator.CreateInstance(typeof(T), new[] { userId }) as T;

            user = (await _context.AddAsync(entity)).Entity;
            await _context.SaveChangesAsync();

            return user;
        }
    }
}