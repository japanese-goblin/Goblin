using System.Collections.Generic;
using System.Linq;

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
        public string Result { get; set; }

        public HelpCommand(List<ICommand> cmds)
        {
            cmds = cmds.OrderBy(x => x.Name).ToList();
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

            Result = $"Общее число команд на данный момент: {cmds.Count(x => !x.IsAdmin) + 1}\n\n{common}\n\n{safu}";
            Result +=
                "\n\nКак установить группу для расписания: зайти на сайт ruz.narfu.ru, выбрать свою ВШ, курс и группу. В полученной ссылке выбрать 4 последние цифры и прописать 'устгр *цифры*'. Например, для ссылки http://ruz.narfu.ru/?timetable&group=1229 необходимо будет прописать 'устгр 1229'.\n\n" +
                "Все команды необходимо писать без ковычек и точно также, как написано здесь. Параметры команды также писать без символа *";
        }

        public void Execute(string param, int id = 0)
        {
        }
    }
}