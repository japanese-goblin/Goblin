using System.Linq;
using System.Threading.Tasks;
using Goblin.Helpers;
using Microsoft.EntityFrameworkCore;
using Narfu;
using Vk.Models.Keyboard;
using Vk.Models.Messages;

namespace Goblin.Bot.Commands
{
    public class Exams : ICommand
    {
        public string Name { get; } = "Экзамены";
        public string Decription { get; } = "Возвращает список экзаменов/зачетов/интернет-экзаменов";
        public string Usage { get; } = "Экзамены";
        public string[] Allias { get; } = {"экзамены"};
        public Category Category { get; } = Category.SAFU;
        public bool IsAdmin { get; } = false;

        public string Message { get; set; }
        public Keyboard Keyboard { get; set; }

        public async Task Execute(Message msg)
        {
            var user = await DbHelper.Db.Users.FirstOrDefaultAsync(x => x.Vk == msg.FromId);

            Message = await StudentsSchedule.GetExams(user.Group);
        }

        public bool CanExecute(Message msg)
        {
            var user = DbHelper.Db.Users.First(x => x.Vk == msg.FromId);
            if (user.Group == 0)
            {
                Message = "Чтобы воспользоваться командой, установи группу командой 'устгр *номер группы*'";
                return false;
            }

            return true;
        }
    }
}