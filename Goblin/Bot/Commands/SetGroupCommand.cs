using System.Collections.Generic;
using System.Linq;
using Goblin.Helpers;
using Goblin.Models;

namespace Goblin.Bot.Commands
{
    public class SetGroupCommand : ICommand
    {
        public string Name => "Устгр *циферки*";
        public string Decription => "Установить группу для получения расписания";
        public string Usage => "Устгр 351617";
        public List<string> Allias => new List<string> {"устгр"};
        public Category Category => Category.SAFU;
        public bool IsAdmin => false;
        public string Result { get; set; }

        private MainContext db = new MainContext();

        public void Execute(string param, int id = 0)
        {
            var group = short.Parse(param);

            db.Users.First(x => x.Vk == id).Group = group;
            db.SaveChanges();
            Result = $"Группа успешно установлена на {group}!";
        }

        public bool CanExecute(string param, int id = 0)
        {
            if (int.TryParse(param, out var i) && ScheduleHelper.IsCorrectGroup(i))
            {
                Result = "Ошибочка. Номер группы - положительно число без знаков (6 цифр)";
                return false;
            }

            return true;
        }
    }
}