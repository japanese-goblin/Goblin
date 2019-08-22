using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Goblin.Application.Abstractions;
using Goblin.Application.Extensions;
using Goblin.Application.Results;
using Goblin.Application.Results.Failed;
using Goblin.Application.Results.Success;
using Goblin.Domain.Entities;
using Newtonsoft.Json;
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
                return await ExecuteKeyboardCommand(msg, user);
            }

            return await ExecuteTextCommand(msg, user);
        }

        private async Task<IResult> ExecuteTextCommand(Message msg, BotUser user)
        {
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

            return new CommandNotFoundResult();
        }

        private async Task<IResult> ExecuteKeyboardCommand(Message msg, BotUser user)
        {
            var record = JsonConvert.DeserializeObject<Dictionary<string, string>>(msg.Payload).FirstOrDefault();
            foreach(var command in _keyboardCommands)
            {
                if(record.Key.Contains(command.Trigger))
                {
                    return await command.Execute(msg, user);
                }
            }

            return new CommandNotFoundResult();
        }
    }
}