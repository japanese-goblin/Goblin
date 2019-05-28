using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Goblin.Bot.Enums;
using Goblin.Bot.Models;
using Vk.Models.Messages;

namespace Goblin.Bot.Commands
{
    public class Help : IInfoCommand
    {
        private readonly IEnumerable<ICommand> _commands;
        public string Name { get; } = "Команды";
        public string Decription { get; } = "Описание команд";
        public string Usage { get; } = "Команды";
        public string[] Allias { get; } = { "help", "команды", "помощь", "помоги", "хелп" };
        public CommandCategory Category { get; } = CommandCategory.Common;
        public bool IsAdmin { get; } = false;

        public Help(IEnumerable<ICommand> commands)
        {
            _commands = commands;
        }

        public Task<CommandResponse> Execute(Message msg)
        {
            var cmds = _commands.Where(x => !x.IsAdmin).OrderBy(x => x.Name).ToList();

            //TODO: edit it (GroupBy?)
            var com = 1;
            var saf = 1;
            var strBuilder = new StringBuilder();
            strBuilder.AppendLine("Общие команды:");

            foreach(var cmd in cmds.Where(x => x.Category == CommandCategory.Common))
            {
                strBuilder.AppendFormat("{0}) {1} - {2}",
                                        com++, cmd.Name, cmd.Decription)
                          .AppendLine();
            }

            strBuilder.AppendLine()
                      .AppendLine("Команды для сафу:");

            foreach(var cmd in cmds.Where(x => x.Category == CommandCategory.Safu))
            {
                strBuilder.AppendFormat("{0}) {1} - {2}",
                                        saf++, cmd.Name, cmd.Decription)
                          .AppendLine();
            }

            strBuilder.AppendLine()
                      .AppendLine("Через звездочку (*) указаны параметры команд (например, для команды «устгр *номер группы*» параметром является номер группы). Звездочки писать не нужно!!!")
                      .AppendLine("Например, для использования команды «устгр» необходимо отправить сообщение «устгр 351010»")
                      .AppendLine()
                      .AppendLine("Более подробная информация с примерами представлена здесь - https://vk.com/@-146048760-commands");
            return Task.FromResult(new CommandResponse
            {
                Text = strBuilder.ToString()
            });
        }

        public (bool Success, string Text) CanExecute(Message msg)
        {
            return (true, "");
        }
    }
}