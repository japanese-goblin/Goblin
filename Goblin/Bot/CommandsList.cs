using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Goblin.Bot.Commands;
using Goblin.Helpers;

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
                new Merge(),
                new KeyboardCommand(),
                new Random(),
                new FindTeacher(),
                new TeacherSchedule()
            };

            Commands.Add(new Help(Commands));
        }

        public static async Task<(string Message, Vk.Models.Keyboard.Keyboard Keyboard)> ExecuteCommand(string message, long userId)
        {
            var split = message.Split(' ', 2);
            var comm = split[0].ToLower();
            var param = split.Length > 1 ? split[1] : "";
            var result = ErrorMessage;
            Vk.Models.Keyboard.Keyboard kb = null;
            foreach (var command in Commands)
            {
                if (!command.Allias.Contains(comm)) continue;

                if (command.IsAdmin && !DbHelper.GetAdmins().Any(x => x == userId)) continue;

                if (command.CanExecute(param, userId))
                {
                    await command.Execute(param, userId);
                }

                result = command.Message;
                kb = command.Keyboard;
                break;
            }

            return (result, kb);
        }
    }
}