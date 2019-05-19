using System.Linq;
using System.Threading.Tasks;
using Goblin.Bot.Enums;
using Goblin.Bot.Models;
using Goblin.Persistence;
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
        public CommandCategory Category { get; } = CommandCategory.Safu;
        public bool IsAdmin { get; } = false;

        private readonly ApplicationDbContext _db;
        private readonly NarfuService _service;

        public Exams(ApplicationDbContext db, NarfuService service)
        {
            _db = db;
            _service = service;
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

            var user = await _db.BotUsers.FirstOrDefaultAsync(x => x.Vk == msg.FromId);
            return new CommandResponse
            {
                Text = await _service.Students.GetExamsAsString(user.Group)
            };
        }

        public (bool Success, string Text) CanExecute(Message msg)
        {
            var user = _db.BotUsers.First(x => x.Vk == msg.FromId);
            if(user.Group == 0)
            {
                return (false, "Ошибка. Группа не установлена. " +
                               "Чтобы воспользоваться командой, установи группу командой 'устгр *номер группы*' (например - устгр 353535)");
            }

            return (true, "");
        }
    }
}