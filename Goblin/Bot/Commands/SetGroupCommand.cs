using Goblin.Helpers;
using Goblin.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Goblin.Bot.Commands
{
    public class SetGroupCommand : ICommand
    {
        public string Name => "Устгр *циферки*";
        public string Decription => "Установить группу для получения расписания";
        public string Usage => "Устгр 351617";
        public List<string> Allias => new List<string> { "устгр" };
        public Category Category => Category.SAFU;
        public bool IsAdmin => false;
        public string Result { get; set; }

        private MainContext db = new MainContext();

        public async Task Execute(string param, int id = 0)
        {
            var group = int.Parse(param);
            var gr = ScheduleHelper.GetGroupByRealId(group);

            var user = await db.Users.FirstAsync(x => x.Vk == id);
            user.Group = group;
            await db.SaveChangesAsync();
            Result = $"Группа успешно установлена на {group} ({gr.Name})!";
        }

        public bool CanExecute(string param, int id = 0)
        {
            if (int.TryParse(param, out var i) && ScheduleHelper.IsCorrectGroup(i))
            {
                return true;
            }
            else
            {
                Result = "Ошибочка. Номер группы - положительно число без знаков (6 цифр)";
                return false;
            }
        }
    }
}