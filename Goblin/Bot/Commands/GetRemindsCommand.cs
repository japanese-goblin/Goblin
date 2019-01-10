using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Goblin.Helpers;
using Microsoft.EntityFrameworkCore;
using Vk.Models.Keyboard;

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
        public Keyboard Keyboard { get; set; }

        public async Task Execute(string param, long id = 0)
        {
            var ureminds = await DbHelper.Db.Reminds.Where(x => x.VkID == id).OrderBy(x => x.Date).ToListAsync();
            if (!ureminds.Any())
            {
                Message = "Напоминаний нет.";
                return;
            }

            Message = "Список напоминаний: \n" + string.Join("\n",
                          ureminds.Select(rem => $"{rem.Date:dd.MM.yyyy (dddd) HH:mm} - {rem.Text}"));
        }

        public bool CanExecute(string param, long id = 0)
        {
            return true;
        }
    }
}