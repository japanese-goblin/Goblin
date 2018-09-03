using Goblin.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Goblin.Bot.Commands
{
    public class UnsetMailingCommand : ICommand
    {
        public string Name { get; } = "Отписка *расписание/погода*";
        public string Decription { get; } = "Отписка от рассылки расписания/погоды (ЧТО-ТО ОДНО ЗА РАЗ)";
        public string Usage { get; } = "Отписка погода";
        public List<string> Allias { get; } = new List<string> { "отписка" };
        public Category Category { get; } = Category.Common;
        public bool IsAdmin { get; } = false;

        public string Result { get; set; }

        private MainContext db = new MainContext();

        public async Task Execute(string param, int id = 0)
        {
            User user;
            switch (param)
            {
                case "погода":
                    user = await db.Users.FirstAsync(x => x.Vk == id);
                    user.Weather = false;
                    Result = "Ты отписался от рассылки погоды :с";
                    break;

                case "расписание":
                    user = await db.Users.FirstAsync(x => x.Vk == id);
                    user.Schedule = false;
                    Result = "Ты отписался от рассылки расписания :с";
                    break;

                default:
                    Result = "Нет такого выбора";
                    break;
            }
            if(db.ChangeTracker.HasChanges())
                await db.SaveChangesAsync();
        }

        public bool CanExecute(string param, int id = 0)
        {
            if (string.IsNullOrEmpty(param))
            {
                Result = "А на что подписаться? Укажи 'погода' либо 'расписание'";
                return false;
            }

            return true;
        }
    }
}