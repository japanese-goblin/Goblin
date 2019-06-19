using System.Threading.Tasks;
using Goblin.Bot.Enums;
using Goblin.Bot.Models;
using Goblin.Domain.Entities;
using Goblin.Persistence;
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
        public CommandCategory Category => CommandCategory.Safu;
        public bool IsAdmin => false;

        private readonly BotDbContext _db;
        private readonly NarfuService _service;

        public SetGroup(BotDbContext db, NarfuService service)
        {
            _db = db;
            _service = service;
        }

        public async Task<CommandResponse> Execute(Message msg, BotUser user)
        {
            var canExecute = CanExecute(msg, user);
            if(!canExecute.Success)
            {
                return new CommandResponse
                {
                    Text = canExecute.Text
                };
            }

            var group = int.Parse(msg.GetParams());
            var gr = _service.Students.GetGroupByRealId(group);

            user.Group = group;
            await _db.SaveChangesAsync();

            return new CommandResponse
            {
                Text = $"Группа успешно установлена на {group} ({gr.Name})!"
            };
        }

        public (bool Success, string Text) CanExecute(Message msg, BotUser user)
        {
            if(int.TryParse(msg.GetParams(), out var i) && _service.Students.IsCorrectGroup(i))
            {
                return (true, "");
            }

            return (false, "Ошибка. Группа с таким номером не найдена");
        }
    }
}