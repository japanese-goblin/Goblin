using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Goblin.Application.Abstractions;
using Goblin.Application.Extensions;
using Goblin.Application.Results.Failed;
using Goblin.Application.Results.Success;
using Goblin.Domain.Entities;
using Newtonsoft.Json;
using Serilog;
using VkNet.Model;

namespace Goblin.Application
{
    public class CommandsService
    {
        private readonly IEnumerable<IKeyboardCommand> _keyboardCommands;
        private readonly ILogger _logger;
        private readonly IEnumerable<ITextCommand> _textCommands;

        public CommandsService(IEnumerable<ITextCommand> textCommands,
                               IEnumerable<IKeyboardCommand> keyboardCommands)
        {
            _textCommands = textCommands;
            _keyboardCommands = keyboardCommands;
            _logger = Log.ForContext<CommandsService>();
        }

        public async Task<IResult> ExecuteCommand(Message msg, BotUser user)
        {
            if(!string.IsNullOrWhiteSpace(msg.Payload))
            {
                return await ExecuteKeyboardCommand(msg, user);
            }

            return await ExecuteTextCommand(msg, user);
        }

        private async Task<IResult> ExecuteTextCommand(Message msg, BotUser user)
        {
            _logger.Debug("Обработка текстовой команды");
            var cmdName = msg.GetCommand();

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
                var result = await command.Execute(msg, user);
                if(result is FailedResult failedExecuteResult)
                {
                    _logger.Debug("Команда вернула {0} результат", failedExecuteResult.GetType());
                    return failedExecuteResult;
                }

                _logger.Debug("Команда выполнена успешно");

                return result as SuccessfulResult;
            }

            return new CommandNotFoundResult();
        }

        private async Task<IResult> ExecuteKeyboardCommand(Message msg, BotUser user)
        {
            _logger.Debug("Обработка команды с клавиатуры");
            var record = JsonConvert.DeserializeObject<Dictionary<string, string>>(msg.Payload).FirstOrDefault();
            foreach(var command in _keyboardCommands)
            {
                if(record.Key.Contains(command.Trigger))
                {
                    _logger.Debug("Выполнение команды с клавиатуры {0}", command.GetType());
                    return await command.Execute(msg, user);
                }
            }

            return new CommandNotFoundResult();
        }
    }
}