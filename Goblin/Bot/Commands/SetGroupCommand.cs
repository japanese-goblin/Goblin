using System.Collections.Generic;
using System.Threading.Tasks;
using Goblin.Helpers;
using Goblin.Schedule;
using Goblin.Vk.Models;
using Microsoft.EntityFrameworkCore;

namespace Goblin.Bot.Commands
{
    public class SetGroupCommand : ICommand
    {
        public string Name => "Устгр *номер группы*";
        public string Decription => "Установить группу для получения расписания";
        public string Usage => "Устгр 351617";
        public List<string> Allias => new List<string> {"устгр"};
        public Category Category => Category.SAFU;
        public bool IsAdmin => false;

        public string Message { get; set; }
        public Keyboard Keyboard { get; set; }

        public async Task Execute(string param, long id = 0)
        {
            var group = int.Parse(param);
            var gr = StudentsSchedule.GetGroupByRealId(group);

            var user = await DbHelper.Db.Users.FirstAsync(x => x.Vk == id);
            user.Group = group;
            await DbHelper.Db.SaveChangesAsync();
            Message = $"Группа успешно установлена на {group} ({gr.Name})!";
        }

        public bool CanExecute(string param, long id = 0)
        {
            if (int.TryParse(param, out var i) && StudentsSchedule.IsCorrectGroup(i))
            {
                return true;
            }
            else
            {
                Message = "Ошибочка. Номер группы - положительно число без знаков (6 цифр)";
                return false;
            }
        }
    }
}