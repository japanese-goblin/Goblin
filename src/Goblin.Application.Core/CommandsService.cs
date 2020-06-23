using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Goblin.Application.Core.Abstractions;
using Goblin.Application.Core.Results.Failed;
using Goblin.Domain.Abstractions;
using Newtonsoft.Json;
using Serilog;

namespace Goblin.Application.Core
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

        public async Task<IResult> ExecuteCommand<T>(IMessage msg, BotUser user) where T : BotUser
        {
            IResult result;
            if(!string.IsNullOrWhiteSpace(msg.Payload))
            {
                result = await ExecuteKeyboardCommand<T>(msg, user);
            }
            else
            {
                result = await ExecuteTextCommand<T>(msg, user);
            }

            result.Keyboard ??= DefaultKeyboards.GetDefaultKeyboard();

            return result;
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

                var isAllowed = command.IsAdminCommand && user.IsAdmin;
                if(!isAllowed)
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
    }
}