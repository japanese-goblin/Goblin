using System.Threading.Tasks;
using Goblin.Helpers;
using Goblin.Models;
using Microsoft.EntityFrameworkCore;
using Vk.Models.Keyboard;
using Vk.Models.Messages;

namespace Goblin.Bot.Commands
{
    public class SetMailing : ICommand
    {
        public string Name { get; } = "Подписка *расписание/погода*";
        public string Decription { get; } = "Подписка на рассылку расписания/погоды (ЧТО-ТО ОДНО ЗА РАЗ)";
        public string Usage { get; } = "Подписка расписание";
        public string[] Allias { get; } = {"подписка"};
        public Category Category { get; } = Category.Common;
        public bool IsAdmin { get; } = false;

        public string Message { get; set; }
        public Keyboard Keyboard { get; set; }

        public async Task Execute(Message msg)
        {
            User user;
            switch (msg.GetParams()) //TODO
            {
                case "погода":
                    user = await DbHelper.Db.Users.FirstAsync(x => x.Vk == msg.FromId);
                    user.Weather = true;
                    Message = "Ты успешно подписался на рассылку погоды!";
                    break;
                case "расписание":
                    user = await DbHelper.Db.Users.FirstAsync(x => x.Vk == msg.FromId);
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

        public bool CanExecute(Message msg)
        {
            if (string.IsNullOrEmpty(msg.GetParams()))
            {
                Message = "А на что подписаться? Укажи 'погода' либо 'расписание'";
                return false;
            }

            return true;
        }
    }
}