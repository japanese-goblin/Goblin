using System.Linq;
using System.Threading.Tasks;
using Goblin.Helpers;
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

        public async Task<CommandResponse> Execute(Message msg)
        {
            var text = "";
            var ureminds = await DbHelper.Db.Reminds.Where(x => x.VkID == msg.FromId).OrderBy(x => x.Date)
                                         .ToListAsync();
            if(!ureminds.Any())
            {
                text = "Напоминаний нет.";
            }
            else
            {
                text = "Список напоминаний: \n" + string.Join("\n",
                                                              ureminds.Select(rem =>
                                                                                  $"{rem.Date:dd.MM.yyyy (dddd) HH:mm} - {rem.Text}"));
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
