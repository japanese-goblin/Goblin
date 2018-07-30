using System.Collections.Generic;
using System.Linq;

namespace Goblin.Bot.Commands
{
    public class SetGroupCommand : ICommand
    {
        public string Name => "Устгр *циферки*";
        public string Decription => "Установить группу для получения расписания";
        public string Usage => "Устгр 3124";
        public List<string> Allias => new List<string> {"устгр"};
        public Category Category => Category.SAFU;
        public bool IsAdmin => false;
        public string Result { get; set; }

        public void Execute(string param, int id = 0)
        {
            var group = short.Parse(param);

            Utils.DB.Users.First(x => x.Vk == id).Group = group;
            Utils.DB.SaveChanges();
            Result = $"Группа успешно установлена на {group}!";
        }

        public bool CanExecute(string param, int id = 0)
        {
            if (!short.TryParse(param, out var group))
            {
                Result = "Ошибочка. Номер группы - положительно число без знаков (4 цифры из ссылки с расписания!!!)";
                return false;
            }

            return true;
        }
    }
}