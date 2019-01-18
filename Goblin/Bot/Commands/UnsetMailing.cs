using System.Threading.Tasks;
using Goblin.Helpers;
using Goblin.Models;
using Microsoft.EntityFrameworkCore;
using Vk.Models.Keyboard;
using Vk.Models.Messages;

namespace Goblin.Bot.Commands
{
    public class UnsetMailing : ICommand
    {
        public string Name { get; } = "Отписка *расписание/погода*";
        public string Decription { get; } = "Отписка от рассылки расписания/погоды (ЧТО-ТО ОДНО ЗА РАЗ)";
        public string Usage { get; } = "Отписка погода";
        public string[] Allias { get; } = {"отписка"};
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
                    user.Weather = false;
                    Message = "Ты отписался от рассылки погоды :с";
                    break;

                case "расписание":
                    user = await DbHelper.Db.Users.FirstAsync(x => x.Vk == msg.FromId);
                    user.Schedule = false;
                    Message = "Ты отписался от рассылки расписания :с";
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