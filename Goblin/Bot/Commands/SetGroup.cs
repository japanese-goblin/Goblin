using Goblin.Helpers;
using Microsoft.EntityFrameworkCore;
using Narfu;
using System.Threading.Tasks;
using Vk.Models.Keyboard;
using Vk.Models.Messages;

namespace Goblin.Bot.Commands
{
    public class SetGroup : ICommand
    {
        public string Name => "Устгр *номер группы*";
        public string Decription => "Установить группу для получения расписания";
        public string Usage => "Устгр 351617";
        public string[] Allias { get; } = { "устгр" };
        public Category Category => Category.SAFU;
        public bool IsAdmin => false;

        public string Message { get; set; }
        public Keyboard Keyboard { get; set; }

        public async Task Execute(Message msg)
        {
            var group = int.Parse(msg.GetParams());
            var gr = StudentsSchedule.GetGroupByRealId(group);

            var user = await DbHelper.Db.Users.FirstAsync(x => x.Vk == msg.FromId);
            user.Group = group;
            await DbHelper.Db.SaveChangesAsync();
            Message = $"Группа успешно установлена на {group} ({gr.Name})!";
        }

        public bool CanExecute(Message msg)
        {
            if (int.TryParse(msg.GetParams(), out var i) && StudentsSchedule.IsCorrectGroup(i))
            {
                return true;
            }

            Message = "Ошибочка. Номер группы - положительно число без знаков (6 цифр)";
            return false;
        }
    }
}