using System.Collections.Generic;
using System.Linq;

namespace Goblin.Bot.Commands
{
    public class SetMailingCommand : ICommand
    {
        public string Name { get; } = "подписка";
        public string Decription { get; } = "Подписка на рассылку расписания/погоды";
        public string Usage { get; } = "Подписка расписание";
        public List<string> Allias { get; } = new List<string>() {"подписка"};
        public Category Category { get; } = Category.Common;
        public bool IsAdmin { get; } = false;

        public string Result { get; set; }

        public void Execute(string param, int id = 0)
        {
            if (string.IsNullOrEmpty(param))
            {
                Result = "А на что подписаться?";
                return;
            }

            switch (param)
            {
                case "погода":
                    Utils.DB.Users.First(x => x.Vk == id).Weather = true;
                    Result = "Ты успешно подписался на рассылку погоды!";
                    break;
                case "расписание":
                    Utils.DB.Users.First(x => x.Vk == id).Schedule = true;
                    Result = "Ты успешно подписался на рассылку расписания!";
                    break;
                default:
                    Result = "Нет такого выбора";
                    break;
            }

            Utils.DB.SaveChanges(); // TODO: не сохранять, если не изменилось?
        }
    }
}