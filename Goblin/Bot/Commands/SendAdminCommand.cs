using System.Collections.Generic;
using System.Threading.Tasks;
using Goblin.Helpers;
using Vk;
using Vk.Models.Keyboard;

namespace Goblin.Bot.Commands
{
    public class SendAdminCommand : ICommand
    {
        public string Name { get; } = "адм *сообщение*";
        public string Decription { get; } = "Отправляет сообщение администраторам бота.";
        public string Usage { get; } = "адм хелп";
        public List<string> Allias { get; } = new List<string> {"адм"};
        public Category Category { get; } = Category.Common;
        public bool IsAdmin { get; } = false;

        public string Message { get; set; }
        public Keyboard Keyboard { get; set; }

        public async Task Execute(string param, long id = 0)
        {
            var username = await Users.GetUserName(id);
            var msg = $"сообщение от @id{id} ({username}):\n\n{param}";
            await Messages.Send(DbHelper.GetAdmins(), msg);
            Message = "Сообщение успешно отправлено!";
        }

        public bool CanExecute(string param, long id = 0)
        {
            if (string.IsNullOrEmpty(param))
            {
                Message = "Введите сообщение для отправки.";
                return false;
            }

            return true;
        }
    }
}