using System.Linq;
using System.Threading.Tasks;
using Goblin.Data.Enums;
using Goblin.Data.Models;
using Microsoft.EntityFrameworkCore;
using Vk.Models.Messages;

namespace Goblin.Bot.Commands
{
    public class GetReminds : ICommand
    {
        public string Name { get; } = "Напоминания";
        public string Decription { get; } = "Возвращает список с созданными напоминаниями";
        public string Usage { get; } = "Напоминания";
        public string[] Allias { get; } = { "напоминания" };
        public Category Category { get; } = Category.Common;
        public bool IsAdmin { get; } = false;

        private readonly MainContext _db;

        public GetReminds(MainContext db)
        {
            _db = db;
        }

        public async Task<CommandResponse> Execute(Message msg)
        {
            var text = "";
            var ureminds = await _db.Reminds.Where(x => x.VkId == msg.FromId)
                                    .OrderBy(x => x.Date).ToListAsync();
            if(!ureminds.Any())
            {
                text = "Напоминаний нет.";
            }
            else
            {
                var selected = ureminds.Select(rem => $"{rem.Date:dd.MM.yyyy (dddd) HH:mm} - {rem.Text}");
                text = "Список напоминаний: \n" + string.Join("\n", selected);
            }

            return new CommandResponse
            {
                Text = text
            };
        }

        public (bool Success, string Text) CanExecute(Message msg)
        {
            return (true, "");
        }
    }
}