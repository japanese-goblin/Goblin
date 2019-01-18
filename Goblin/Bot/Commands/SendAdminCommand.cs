using Goblin.Helpers;
using System.Threading.Tasks;
using Vk;
using Vk.Models.Keyboard;

namespace Goblin.Bot.Commands
{
    public class SendAdminCommand : ICommand
    {
        public string Name { get; } = "Админ *сообщение*";
        public string Decription { get; } = "Отправляет сообщение администраторам бота.";
        public string Usage { get; } = "Админ хелп";
        public string[] Allias { get; } = { "админ" };
        public Category Category { get; } = Category.Common;
        public bool IsAdmin { get; } = false;

        public string Message { get; set; }
        public Keyboard Keyboard { get; set; }

        public async Task Execute(string param, long id = 0)
        {
            var username = await VkApi.Users.GetUserName(id);
            var msg = $"сообщение от @id{id} ({username}):\n\n{param}";
            await VkApi.Messages.Send(DbHelper.GetAdmins(), msg);

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