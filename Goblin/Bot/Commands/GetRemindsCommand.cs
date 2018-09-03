using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Goblin.Models;
using Microsoft.EntityFrameworkCore;

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
        public string Result { get; set; }

        private MainContext db = new MainContext();

        public async Task Execute(string param, int id = 0)
        {
            var reminds = "Список напоминаний:\n";
            var ureminds = await db.Reminds.Where(x => x.VkID == id).OrderBy(x => x.Date).ToListAsync();
            if (!ureminds.Any())
            {
                Result = "Напоминаний нет.";
                return;
            }

            foreach (var rem in ureminds)
            {
                var d = rem.Date;
                reminds += $"{d:dd.MM.yyyy HH:mm} - {rem.Text}\n";
            }

            Result = reminds;
        }

        public bool CanExecute(string param, int id = 0)
        {
            return true;
        }
    }
}