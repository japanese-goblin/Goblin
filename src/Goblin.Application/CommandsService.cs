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
        private readonly IEnumerable<ITextCommand> _commands;

        public CommandsService(IEnumerable<ITextCommand> commands)
        {
            _commands = commands;
        }

        public async Task<IResult> ExecuteCommand(Message msg, BotUser user)
        {
            var cmdName = msg.GetCommand();

            foreach(var command in _commands)
            {
                if(!command.Aliases.Contains(cmdName))
                {
                    continue;
                }

                if(command.IsAdminCommand && !user.IsAdmin)
                {
                    continue;
                }

                var result = await command.Execute(msg);
                if(result is FailedResult failedExecuteResult)
                {
                    return failedExecuteResult;
                }

                return result as SuccessfulResult;
            }

            return new FailedResult(new List<string>
            {
                "команда не найдена. Проверьте правильность написания команды."
            });
        }
    }
}