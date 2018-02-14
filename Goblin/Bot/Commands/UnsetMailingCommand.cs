using System.Collections.Generic;
using System.Linq;

namespace Goblin.Bot.Commands
{
    public class UnsetMailingCommand : ICommand
    {
        public string Name { get; } = "отписка";
        public string Decription { get; } = "Отписка от рассылки расписания/погоды";
        public string Usage { get; } = "Отписка погода";
        public List<string> Allias { get; } = new List<string>() { "отписка" };
        public Category Category { get; } = Category.Common;
        public bool IsAdmin { get; } = false;

        public string Result { get; set; }

        public void Execute(string param, int id = 0)
        {
            if (string.IsNullOrEmpty(param))
            {
                Result = "А от чего отписаться?";
                return;
            }

            switch (param)
            {
                case "погода":
                    Utils.DB.Users.First(x => x.Vk == id).Weather = false;
                    Result = "Ты отписался от рассылки погоды :с";
                    break;

                case "расписание":
                    Utils.DB.Users.First(x => x.Vk == id).Schedule = false;
                    Result = "Ты отписался от рассылки расписания :с";
                    break;

                default:
                    Result = "Нет такого выбора";
                    break;
            }

            Utils.DB.SaveChanges(); // TODO: не сохранять, если не изменилось?
        }
    }
}