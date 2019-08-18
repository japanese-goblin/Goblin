using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Goblin.Application.Abstractions;
using Goblin.Application.Extensions;
using Goblin.Application.Results;
using Goblin.Domain.Entities;
using VkNet.Model;

namespace Goblin.Application
{
    public class CommandsService
    {
        private readonly IEnumerable<ITextCommand> _textCommands;
        private readonly IEnumerable<IKeyboardCommand> _keyboardCommands;

        public CommandsService(IEnumerable<ITextCommand> textCommands,
                               IEnumerable<IKeyboardCommand> keyboardCommands)
        {
            _textCommands = textCommands;
            _keyboardCommands = keyboardCommands;
        }

        public async Task<IResult> ExecuteCommand(Message msg, BotUser user)
        {
            if(!string.IsNullOrWhiteSpace(msg.Payload))
            {
                return await ExecuteKeyboard(msg, user);
            }
            
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

                var result = await command.Execute(msg, user);
                if(result is FailedResult failedExecuteResult)
                {
                    return failedExecuteResult;
                }

                return result as SuccessfulResult;
            }

            return new FailedResult("команда не найдена. Проверьте правильность написания команды.");
        }

        private async Task<IResult> ExecuteKeyboard(Message msg, BotUser user)
        {
            foreach(var command in _keyboardCommands)
            {
                if(msg.Payload.Contains(command.Trigger))
                {
                    return await command.Execute(msg, user);
                }
            }
            
            return new FailedResult("Команда не найдена.");
        }
    }
}