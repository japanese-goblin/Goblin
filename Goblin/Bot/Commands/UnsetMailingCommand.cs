using System.Collections.Generic;
using System.Linq;
using Goblin.Models;

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

        public string Result { get; set; }

        private MainContext db = new MainContext();

        public void Execute(string param, int id = 0)
        {
            switch (param)
            {
                case "погода":
                    db.Users.First(x => x.Vk == id).Weather = false;
                    Result = "Ты отписался от рассылки погоды :с";
                    break;

                case "расписание":
                    db.Users.First(x => x.Vk == id).Schedule = false;
                    Result = "Ты отписался от рассылки расписания :с";
                    break;

                default:
                    Result = "Нет такого выбора";
                    break;
            }

            db.SaveChanges(); // TODO: не сохранять, если не изменилось?
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