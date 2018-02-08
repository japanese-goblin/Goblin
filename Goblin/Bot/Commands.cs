using System.Collections.Generic;
using Goblin.Bot.Commands;

namespace Goblin.Bot
{
    public static class CommandsList
    {
        private static List<ICommand> Commands = new List<ICommand>()
        {
            new RandomCommand(),
        };

        public static string ExecuteCommand(string message)
        {
            Commands.Add(new HelpCommand(Commands));
            var split = message.Split(' ', 2);
            var comm = split[0];
            var param = split.Length > 1 ? split[1] : "";
            var result = "неизвестность";
            foreach (var command in Commands)
            {
                if (command.Allias.Contains(comm))
                {
                    command.Execute(param);
                    result = command.Result;
                    break;
                }
            }

            return result;
        }
    }
}