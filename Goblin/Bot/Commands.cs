using System.Collections.Generic;
using Goblin.Bot.Commands;

namespace Goblin.Bot
{
    public static class CommandsList
    {
        private static List<ICommand> Commands = new List<ICommand>()
        {
            new RandomCommand(),
            new ChanceCommand(),
            new DebugCommand(),
            new SetGroupCommand(),
            new SetCityCommand(),
            new WeatherCommand(),
            new GetRemindsCommand(),
            new AddRemindCommand(),
        };

        public static string ExecuteCommand(string message, int id)
        {
            Commands.Add(new HelpCommand(Commands));
            var split = message.Split(' ', 2);
            var comm = split[0].ToLower();
            var param = split.Length > 1 ? split[1] : "";
            var result = "неизвестность";
            foreach (var command in Commands)
            {
                if (command.Allias.Contains(comm))
                {
                    if (command.IsAdmin && !Utils.DevelopersID.Contains(id)) break;
                    command.Execute(param, id);
                    result = command.Result;
                    break;
                }
            }

            return result;
        }
    }
}