using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Goblin.Helpers;
using Microsoft.EntityFrameworkCore;
using Narfu;
using Vk.Models.Keyboard;

namespace Goblin.Bot.Commands
{
    public class ExamsCommand : ICommand
    {
        public string Name { get; } = "Экзамены";
        public string Decription { get; } = "Возвращает список экзаменов/зачетов/интернет-экзаменов";
        public string Usage { get; } = "Экзамены";
        public List<string> Allias { get; } = new List<string> {"экзамены"};
        public Category Category { get; } = Category.SAFU;
        public bool IsAdmin { get; } = false;

        public string Message { get; set; }
        public Keyboard Keyboard { get; set; }

        public async Task Execute(string param, long id = 0)
        {
            var user = await DbHelper.Db.Users.FirstOrDefaultAsync(x => x.Vk == id);

            Message = await StudentsSchedule.GetExams(user.Group);
        }

        public bool CanExecute(string param, long id = 0)
        {
            var user = DbHelper.Db.Users.First(x => x.Vk == id);
            if (user.Group == 0)
            {
                Message = "Чтобы воспользоваться командой, установи группу командой 'устгр *номер группы*'";
                return false;
            }

            return true;
        }
    }
}