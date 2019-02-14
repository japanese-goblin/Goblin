using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Goblin.Data.Models;
using Vk.Models.Messages;

namespace Goblin.Bot
{
    public class CommandExecutor
    {
        public static string ErrorMessage = "Ошибочка, проверьте правильность написания команды!";

        private readonly IEnumerable<ICommand> _commands;
        private readonly MainContext _db;

        public CommandExecutor(MainContext db, IEnumerable<ICommand> commands)
        {
            _db = db;
            _commands = commands;
        }

        public async Task<CommandResponse> ExecuteCommand(Message msg)
        {
            var split = msg.Text.Split(' ', 2);
            var comm = split[0].ToLower();
            var response = new CommandResponse();

            foreach(var command in _commands)
            {
                if(!command.Allias.Contains(comm)) continue;

                if(command.IsAdmin && _db.GetAdmins().All(x => x != msg.FromId)) continue;

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