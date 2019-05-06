using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Goblin.Bot.Models;
using Goblin.Persistence;
using Vk.Models.Messages;

namespace Goblin.Bot
{
    public class CommandExecutor
    {
        public static string ErrorMessage = "Ошибочка, проверьте правильность написания команды!";

        private readonly IEnumerable<ICommand> _commands;
        private readonly ApplicationDbContext _db;

        public CommandExecutor(ApplicationDbContext db, IEnumerable<ICommand> commands)
        {
            _db = db;
            _commands = commands;
        }

        public async Task<CommandResponse> ExecuteCommand(Message msg)
        {
            msg.Text = Regex.Replace(msg.Text, @"\s+", " ").Trim();

            var split = msg.Text.Split(' ', 2);
            var comm = split[0].ToLower();
            var response = new CommandResponse();

            foreach(var command in _commands)
            {
                if(!command.Allias.Contains(comm))
                {
                    continue;
                }

                if(command.IsAdmin && _db.GetAdmins().All(x => x != msg.FromId))
                {
                    continue;
                }

                response = await command.Execute(msg);
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