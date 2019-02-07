using System.Threading.Tasks;
using Goblin.Helpers;
using Goblin.Models;
using Microsoft.EntityFrameworkCore;
using Narfu;
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

        private readonly MainContext _db;
        public SetGroup(MainContext db)
        {
            _db = db;
        }

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

            var group = int.Parse(msg.GetParams());
            var gr = StudentsSchedule.GetGroupByRealId(group);

            var user = await _db.Users.FirstAsync(x => x.Vk == msg.FromId);
            user.Group = group;
            await _db.SaveChangesAsync();

            return new CommandResponse
            {
                Text = $"Группа успешно установлена на {group} ({gr.Name})!"
            };
        }

        public (bool Success, string Text) CanExecute(Message msg)
        {
            if(int.TryParse(msg.GetParams(), out var i) && StudentsSchedule.IsCorrectGroup(i))
            {
                return (true, "");
            }

            return (false, "Ошибка. Группа с таким номером не найдена");
        }
    }
}
