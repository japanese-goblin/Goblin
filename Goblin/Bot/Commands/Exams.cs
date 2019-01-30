using System.Linq;
using System.Threading.Tasks;
using Goblin.Helpers;
using Microsoft.EntityFrameworkCore;
using Narfu;
using Vk.Models.Messages;

namespace Goblin.Bot.Commands
{
    public class Exams : ICommand
    {
        public string Name { get; } = "Экзамены";
        public string Decription { get; } = "Возвращает список экзаменов/зачетов/интернет-экзаменов";
        public string Usage { get; } = "Экзамены";
        public string[] Allias { get; } = { "экзамены" };
        public Category Category { get; } = Category.SAFU;
        public bool IsAdmin { get; } = false;

        public async Task<CommandResponse> Execute(Message msg)
        {
            var canExecute = CanExecute(msg);
            if(!canExecute.Success)
            {
                return new CommandResponse
                {
                    Text = canExecute.Text
                };
            }

            var user = await DbHelper.Db.Users.FirstOrDefaultAsync(x => x.Vk == msg.FromId);
            return new CommandResponse
            {
                Text = await StudentsSchedule.GetExams(user.Group)
            };
        }

        public (bool Success, string Text) CanExecute(Message msg)
        {
            var user = DbHelper.Db.Users.First(x => x.Vk == msg.FromId);
            if(user.Group == 0)
            {
                return (false, "Ошибка. Группа не установлена. " +
                               "Чтобы воспользоваться командой, установи группу командой 'устгр *номер группы*' (например - устгр 353535)");
            }

            return (true, "");
        }
    }
}
