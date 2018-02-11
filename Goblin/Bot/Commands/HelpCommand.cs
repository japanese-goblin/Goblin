using System.Collections.Generic;
using System.Linq;

namespace Goblin.Bot.Commands
{
    public class HelpCommand : ICommand
    {
        public string Name { get; } = "Команды";
        public string Decription { get; } = "описание команд";
        public string Usage { get; } = "команды";
        public List<string> Allias { get; } = new List<string>() {"help", "команды", "помощь", "помоги"};
        public Category Category { get; } = Category.Common;
        public bool IsAdmin { get; } = false;
        public string Result { get; set; }

        public HelpCommand(List<ICommand> cmds)
        {
            //TODO: edit it (GrouBy?)
            var common = "Общие команды:\n";
            byte com = 1;
            var safu = "Команды для САФУ:\n";
            byte saf = 1;
            foreach (var cmd in cmds.Where(x => x.Category == Category.Common && !x.IsAdmin))
            {
                common += $"{com++}) {cmd.Name} - {cmd.Decription}\nПример использования команды: {cmd.Usage}\n";
            }
            foreach (var cmd in cmds.Where(x => x.Category == Category.SAFU && !x.IsAdmin))
            {
                safu += $"{saf++}) {cmd.Name} - {cmd.Decription}\nПример использования команды: {cmd.Usage}\n";
            }

            Result = $"Общее число команд на данный момент: {cmds.Count(x => !x.IsAdmin) + 1}\n\n{common}\n\n{safu}";
        }

        public void Execute(string param, int id = 0)
        {
            
        }
    }
}