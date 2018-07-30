using System.Collections.Generic;

namespace Goblin.Bot.Commands
{
    public class SendAdminCommand : ICommand
    {
        public string Name { get; } = "адм *сообщение*";

        public string Decription { get; } =
            "Отправляет сообщение администратору. Использовать при возникновении ошибок или еще чего-то";

        public string Usage { get; } = "адм ПАМАГИТЕ";
        public List<string> Allias { get; } = new List<string>() {"адм"};
        public Category Category { get; } = Category.Common;
        public bool IsAdmin { get; } = false;
        public string Result { get; set; }

        public void Execute(string param, int id = 0)
        {
            var msg = $"сообщение от @id{id} (id{id})\n{param}";
            Utils.SendMessage(Utils.DevelopersID, msg);
        }

        public bool CanExecute(string param, int id = 0)
        {
            if (string.IsNullOrEmpty(param))
            {
                Result = "Введите сообщение для отправки.";
                return false;
            }

            return true;
        }
    }
}