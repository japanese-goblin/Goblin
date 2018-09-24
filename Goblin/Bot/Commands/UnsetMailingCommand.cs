using System.Collections.Generic;
using System.Threading.Tasks;
using Goblin.Helpers;
using Goblin.Models;
using Microsoft.EntityFrameworkCore;
using VkNet.Model.Keyboard;

namespace Goblin.Bot.Commands
{
    public class UnsetMailingCommand : ICommand
    {
        public string Name { get; } = "Отписка *расписание/погода*";
        public string Decription { get; } = "Отписка от рассылки расписания/погоды (ЧТО-ТО ОДНО ЗА РАЗ)";
        public string Usage { get; } = "Отписка погода";
        public List<string> Allias { get; } = new List<string> {"отписка"};
        public Category Category { get; } = Category.Common;
        public bool IsAdmin { get; } = false;

        public string Message { get; set; }
        public MessageKeyboard Keyboard { get; set; }

        public async Task Execute(string param, int id = 0)
        {
            User user;
            switch (param)
            {
                case "погода":
                    user = await DbHelper.Db.Users.FirstAsync(x => x.Vk == id);
                    user.Weather = false;
                    Message = "Ты отписался от рассылки погоды :с";
                    break;

                case "расписание":
                    user = await DbHelper.Db.Users.FirstAsync(x => x.Vk == id);
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