using System.Collections.Generic;
using Goblin.Bot.Commands;

namespace Goblin.Bot
{
    public static class CommandsList
    {
        private static readonly List<ICommand> Commands = new List<ICommand>()
        {
            new RandomCommand(),
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
            new ExamsCommand()
        };

        public static string ExecuteCommand(string message, int id)
        {
            var split = message.Split(' ', 2);
            var comm = split[0].ToLower();
            var param = split.Length > 1 ? split[1] : "";
            var result = "Ошибочка, проверьте правильность написания команды!";
            lock (Commands) // TODO: ????
            {
                Commands.Add(new HelpCommand(Commands)); // TODO: ????
                foreach (var command in Commands)
                {
                    if (!command.Allias.Contains(comm)) continue;
                    if (command.IsAdmin && !Utils.DevelopersID.Contains(id)) continue;
                    command.Execute(param, id);
                    result = command.Result;
                    break;
                }
            }
            return result;
        }
    }
}