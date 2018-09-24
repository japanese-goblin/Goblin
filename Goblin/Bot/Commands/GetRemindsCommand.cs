using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Goblin.Helpers;
using Microsoft.EntityFrameworkCore;
using VkNet.Model.Keyboard;

namespace Goblin.Bot.Commands
{
    public class GetRemindsCommand : ICommand
    {
        public string Name { get; } = "Напоминания";
        public string Decription { get; } = "Возвращает список с созданными напоминаниями";
        public string Usage { get; } = "Напоминания";
        public List<string> Allias { get; } = new List<string> {"напоминания"};
        public Category Category { get; } = Category.Common;
        public bool IsAdmin { get; } = false;

        public string Message { get; set; }
        public MessageKeyboard Keyboard { get; set; }

        public async Task Execute(string param, int id = 0)
        {
            var reminds = "Список напоминаний:\n";
            var ureminds = await DbHelper.Db.Reminds.Where(x => x.VkID == id).OrderBy(x => x.Date).ToListAsync();
            if (!ureminds.Any())
            {
                Message = "Напоминаний нет.";
                return;
            }

            foreach (var rem in ureminds)
            {
                var d = rem.Date;
                reminds += $"{d:dd.MM.yyyy HH:mm} - {rem.Text}\n";
            }

            Message = reminds;
        }

        public bool CanExecute(string param, int id = 0)
        {
            return true;
        }
    }
}