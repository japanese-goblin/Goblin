using System.Collections.Generic;
using System.Linq;

namespace Goblin.Bot.Commands
{
    public class HelpCommand : ICommand
    {
        public string Name { get; } = "Команды";
        public string Decription { get; } = "описание команд";
        public List<string> Allias { get; } = new List<string>() {"help", "команды"};
        public Category Category { get; } = Category.Common;
        public bool IsAdmin { get; } = false;
        public string Result { get; set; }

        public HelpCommand(List<ICommand> cmds)
        {
            //TODO: edit it (GrouBy?)
            var common = "Общие команды:\n";
            var safu = "Команды для САФУ:\n";
            foreach (var cmd in cmds.Where(x => x.Category == Category.Common && !x.IsAdmin))
            {
                common += $"{cmd.Name} - {cmd.Decription}\n";
            }
            foreach (var cmd in cmds.Where(x => x.Category == Category.SAFU && !x.IsAdmin))
            {
                safu += $"{cmd.Name} - {cmd.Decription}\n";
            }

            Result = $"Общее число команд на данный момент: {cmds.Count + 1}\n\n{common}\n\n{safu}";
        }

        public void Execute(string param)
        {
            
        }
    }
}