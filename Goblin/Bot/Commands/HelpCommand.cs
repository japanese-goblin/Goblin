using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VkNet.Model.Keyboard;

namespace Goblin.Bot.Commands
{
    public class HelpCommand : ICommand
    {
        public string Name { get; } = "Команды";
        public string Decription { get; } = "Описание команд";
        public string Usage { get; } = "Команды";
        public List<string> Allias { get; } = new List<string> {"help", "команды", "помощь", "помоги", "хелп"};
        public Category Category { get; } = Category.Common;
        public bool IsAdmin { get; } = false;

        public string Message { get; set; }
        public MessageKeyboard Keyboard { get; set; }

        public HelpCommand(List<ICommand> cmds)
        {
            cmds = cmds.Where(x => !x.IsAdmin).OrderBy(x => x.Name).ToList();
            //TODO: edit it (GroupBy?)
            var common = "Общие команды:\n";
            byte com = 1;
            var safu = "Команды для САФУ:\n";
            byte saf = 1;
            foreach (var cmd in cmds.Where(x => x.Category == Category.Common && !x.IsAdmin))
            {
                common += $"{com++}) {cmd.Name} - {cmd.Decription}\nНапример - {cmd.Usage}\n";
            }

            foreach (var cmd in cmds.Where(x => x.Category == Category.SAFU && !x.IsAdmin))
            {
                safu += $"{saf++}) {cmd.Name} - {cmd.Decription}\nНапример - {cmd.Usage}\n";
            }

            Message = $"Общее число команд на данный момент: {cmds.Count(x => !x.IsAdmin)}\n\n" +
                     $"{common}\n\n" +
                     $"{safu}\n\n" +
                     $"Если Вам нужно посмотреть расписание до конца семестра (а не на месяц, как на официальном сайте), то можете зайти сюда (https://equus-cs.herokuapp.com/Schedule), ввести свой номер группы и посмотреть расписание!!\n\n" +
                     "По любым вопросам/предложениям/ошибкам и прочему, прошу писать сюда: @id***REMOVED*** (тык)";
        }

        public async Task Execute(string param, int id = 0) { }

        public bool CanExecute(string param, int id = 0)
        {
            return true;
        }
    }
}