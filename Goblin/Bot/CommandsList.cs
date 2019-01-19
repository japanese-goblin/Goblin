using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Goblin.Bot.Commands;
using Goblin.Helpers;
using Vk.Models.Messages;

namespace Goblin.Bot
{
    public static class CommandsList
    {
        private static readonly List<ICommand> Commands;
        public static string ErrorMessage = "Ошибочка, проверьте правильность написания команды!";

        static CommandsList()
        {
            Commands = new List<ICommand>
            {
                new Flip(),
                new Chance(),
                new Debug(),
                new SetGroup(),
                new SetCity(),
                new Weather(),
                new GetReminds(),
                new AddRemind(),
                new Schedule(),
                new SetMailing(),
                new UnsetMailing(),
                new Exams(),
                new SendAdmin(),
                new KeyboardCommand(),
                new Random(),
                new FindTeacher(),
                new TeacherSchedule(),
                new Quote()
            };

            Commands.Add(new Help(Commands));
        }

        public static async Task<CommandResponse> ExecuteCommand(Message msg)
        {
            var split = msg.Text.Split(' ', 2);
            var comm = split[0].ToLower();
            var response = new CommandResponse();

            foreach (var command in Commands)
            {
                if (!command.Allias.Contains(comm)) continue;

                if (command.IsAdmin && !DbHelper.GetAdmins().Any(x => x == msg.FromId)) continue;

                response = await command.Execute(msg);
                break;
            }

            if (string.IsNullOrEmpty(response.Text))
            {
                response.Text = ErrorMessage;
            }

            return response;
        }
    }
}