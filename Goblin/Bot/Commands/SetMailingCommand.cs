using System.Collections.Generic;
using System.Threading.Tasks;
using Goblin.Helpers;
using Goblin.Models;
using Goblin.Models.Keyboard;
using Microsoft.EntityFrameworkCore;

namespace Goblin.Bot.Commands
{
    public class SetMailingCommand : ICommand
    {
        public string Name { get; } = "Подписка *расписание/погода*";
        public string Decription { get; } = "Подписка на рассылку расписания/погоды (ЧТО-ТО ОДНО ЗА РАЗ)";
        public string Usage { get; } = "Подписка расписание";
        public List<string> Allias { get; } = new List<string> {"подписка"};
        public Category Category { get; } = Category.Common;
        public bool IsAdmin { get; } = false;

        public string Message { get; set; }
        public Keyboard Keyboard { get; set; }

        public async Task Execute(string param, int id = 0)
        {
            User user;
            switch (param)
            {
                case "погода":
                    user = await DbHelper.Db.Users.FirstAsync(x => x.Vk == id);
                    user.Weather = true;
                    Message = "Ты успешно подписался на рассылку погоды!";
                    break;
                case "расписание":
                    user = await DbHelper.Db.Users.FirstAsync(x => x.Vk == id);
                    user.Schedule = true;
                    Message = "Ты успешно подписался на рассылку расписания!";
                    break;
                default:
                    Message = "Нет такого выбора";
                    break;
            }

            if (DbHelper.Db.ChangeTracker.HasChanges())
                await DbHelper.Db.SaveChangesAsync();
        }

        public bool CanExecute(string param, int id = 0)
        {
            if (string.IsNullOrEmpty(param))
            {
                Message = "А на что подписаться? Укажи 'погода' либо 'расписание'";
                return false;
            }

            return true;
        }
    }
}