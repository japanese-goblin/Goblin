using System.Collections.Generic;
using System.Threading.Tasks;
using Goblin.Models;
using Microsoft.EntityFrameworkCore;
using VkNet.Model.Keyboard;

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
        public MessageKeyboard Keyboard { get; set; }

        private MainContext db = new MainContext();

        public async Task Execute(string param, int id = 0)
        {
            User user;
            switch (param)
            {
                case "погода":
                    user = await db.Users.FirstAsync(x => x.Vk == id);
                    user.Weather = true;
                    Message = "Ты успешно подписался на рассылку погоды!";
                    break;
                case "расписание":
                    user = await db.Users.FirstAsync(x => x.Vk == id);
                    user.Schedule = true;
                    Message = "Ты успешно подписался на рассылку расписания!";
                    break;
                default:
                    Message = "Нет такого выбора";
                    break;
            }

            if (db.ChangeTracker.HasChanges())
                await db.SaveChangesAsync();
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