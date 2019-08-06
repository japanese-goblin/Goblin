using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Goblin.Application.Abstractions;
using Goblin.Application.Extensions;
using Goblin.Application.Results;
using Goblin.Domain.Entities;
using VkNet.Abstractions;
using VkNet.Model;
using VkNet.Model.RequestParams;

namespace Goblin.Application
{
    public class CommandsService
    {
        private readonly IEnumerable<IBotCommand> _commands;
        private readonly IVkApi _vkApi;

        public CommandsService(IEnumerable<IBotCommand> commands, IVkApi vkApi)
        {
            _commands = commands;
            _vkApi = vkApi;
        }

        public async Task ExecuteCommand(Message msg, BotUser user)
        {
            var cmdInfo = msg.GetCommandInfo();
            
            foreach(var command in _commands)
            {
                if(!command.Aliases.Contains(cmdInfo[0])) continue;
                if(command.IsAdminCommand && !user.IsAdmin) continue;

                var isCanExecute = await command.CanExecute(msg);
                if(isCanExecute is FailedResult failedResult)
                {
                    _vkApi.Messages.SendError(failedResult.ToString(), msg.PeerId.Value);
                    
                    return;
                }

                var result = await command.Execute(msg);
                if(result is FailedResult failedExecuteResult)
                {
                    _vkApi.Messages.SendError(failedExecuteResult.ToString(), msg.PeerId.Value);
                    return;
                }

                var resultSuccessful = result as SuccessfulResult;
                _vkApi.Messages.Send(new MessagesSendParams
                {
                    PeerId = msg.PeerId,
                    Message = resultSuccessful.Message,
                    Keyboard = resultSuccessful.Keyboard,
                    Attachments = resultSuccessful.Attachments
                });
            }
        }
    }
}