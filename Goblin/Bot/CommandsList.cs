using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Goblin.Bot.Commands;
using Goblin.Vk.Models;
using Goblin.Vk;

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
                new FlipCommand(),
                new ChanceCommand(),
                new DebugCommand(),
                new SetGroupCommand(),
                new SetCityCommand(),
                new WeatherCommand(),
                new GetRemindsCommand(),
                new AddRemindCommand(),
                new ScheduleCommand(),
                new SetMailingCommand(),
                new UnsetMailingCommand(),
                new ExamsCommand(),
                new SendAdminCommand(),
                new MergeCommand(),
                new KeyboardCommand(),
                new RandomCommand(),
                new FindTeacherCommand(),
                new TeacherScheduleCommand()
            };

            Commands.Add(new HelpCommand(Commands));
        }

        public static async Task<(string Message, Keyboard Keyboard)> ExecuteCommand(string message, int id)
        {
            var split = message.Split(' ', 2);
            var comm = split[0].ToLower();
            var param = split.Length > 1 ? split[1] : "";
            var result = ErrorMessage;
            Keyboard kb = null;
            foreach (var command in Commands)
            {
                if (!command.Allias.Contains(comm)) continue;
                if (command.IsAdmin && !VkMethods.DevelopersID.Any(x => x == id)) continue;
                if (command.CanExecute(param, id))
                    await command.Execute(param, id);
                result = command.Message;
                kb = command.Keyboard;
                break;
            }

            return (result, kb);
        }
    }
}