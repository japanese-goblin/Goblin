using System.Threading.Tasks;
using Goblin.Data.Enums;
using Goblin.Data.Models;
using Vk.Models.Messages;

namespace Goblin.Bot.Commands
{
    //TODO: переделать под DI
    public class Help : ICommand
    {
        public string Name { get; } = "Команды";
        public string Decription { get; } = "Описание команд";
        public string Usage { get; } = "Команды";
        public string[] Allias { get; } = { "help", "команды", "помощь", "помоги", "хелп" };
        public Category Category { get; } = Category.Common;
        public bool IsAdmin { get; } = false;

        public async Task<CommandResponse> Execute(Message msg)
        {
            return new CommandResponse
            {
                Text = "Список всех доступных команд доступен здесь: https://vk.com/@-146048760-commands"
            };
        }

        public (bool Success, string Text) CanExecute(Message msg)
        {
            return (true, "");
        }
    }
}

/*
    private static string _message = "";

    public Help(IEnumerable<ICommand> cmds)
    {
        if(!string.IsNullOrEmpty(_message)) return;

        cmds = cmds.Where(x => !x.IsAdmin).OrderBy(x => x.Name).ToList();
        //TODO: edit it (GroupBy?)
        var common = "Общие команды:\n";
        byte com = 1;
        var safu = "Команды для САФУ:\n";
        byte saf = 1;
        foreach(var cmd in cmds.Where(x => x.Category == Category.Common && !x.IsAdmin))
        {
            common += $"{com++}) {cmd.Name} - {cmd.Decription}\nНапример - {cmd.Usage}\n";
        }

        foreach(var cmd in cmds.Where(x => x.Category == Category.SAFU && !x.IsAdmin))
        {
            safu += $"{saf++}) {cmd.Name} - {cmd.Decription}\nНапример - {cmd.Usage}\n";
        }

        _message = $"Общее число команд на данный момент: {cmds.Count(x => !x.IsAdmin)}\n\n" +
                   $"{common}\n\n" +
                   $"{safu}\n\n" +
                   "Для просмотра расписания преподавателя, необходимо получить его номер через команду ('найти деменков'). " +
                   "После каждого ФИО и названия кафедры указан пятизначный номер преподавателя, который можно использовать для " +
                   "получения расписания у препода (найти 23512).\n" +
                   // "Если Вам нужно посмотреть расписание до конца семестра (а не на месяц, как на официальном сайте), то можете зайти сюда (https://equus-cs.herokuapp.com/Schedule), ввести свой номер группы и посмотреть расписание!!\n\n" +
                   "По любым вопросам/предложениям/ошибкам и прочему, прошу писать сюда: @id***REMOVED*** (тык)";
    }
 */