using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Goblin.Bot.Models;
using Goblin.Domain.Entities;
using Goblin.Persistence;
using Vk.Models.Messages;

namespace Goblin.Bot
{
    public class CommandExecutor
    {
        public static string ErrorMessage = "Ошибочка, проверьте правильность написания команды!";

        private readonly IEnumerable<ICommand> _commands;
        private readonly IInfoCommand _helpcmd;
        private readonly ApplicationDbContext _db;

        public CommandExecutor(ApplicationDbContext db, IEnumerable<ICommand> commands, IInfoCommand helpcmd)
        {
            _db = db;
            _commands = commands;
            _helpcmd = helpcmd;
        }

        public async Task<CommandResponse> ExecuteCommand(Message msg, BotUser botUser)
        {
            msg.Text = Regex.Replace(msg.Text, @"\s+", " ").Trim();

            var split = msg.Text.Split(' ', 2);
            var comm = split[0].ToLower();
            var response = new CommandResponse();

            if(_helpcmd.Aliases.Contains(comm))
            {
                return await _helpcmd.Execute(msg, botUser);
            }

            foreach(var command in _commands)
            {
                if(!command.Aliases.Contains(comm))
                {
                    continue;
                }

                if(command.IsAdmin && !botUser.IsAdmin)
                {
                    continue;
                }

                response = await command.Execute(msg, botUser);
                break;
            }

            if(string.IsNullOrEmpty(response.Text))
            {
                response.Text = ErrorMessage;
            }

            return response;
        }
    }
}