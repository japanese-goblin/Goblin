using Goblin.Helpers;
using System.Threading.Tasks;
using Vk;
using Vk.Models.Keyboard;
using Vk.Models.Messages;

namespace Goblin.Bot.Commands
{
    public class SendAdmin : ICommand
    {
        public string Name { get; } = "Админ *сообщение*";
        public string Decription { get; } = "Отправляет сообщение администраторам бота.";
        public string Usage { get; } = "Админ хелп";
        public string[] Allias { get; } = { "админ" };
        public Category Category { get; } = Category.Common;
        public bool IsAdmin { get; } = false;

        public string Message { get; set; }
        public Keyboard Keyboard { get; set; }

        public async Task Execute(Message msg)
        {
            var username = await VkApi.Users.GetUserName(msg.FromId);
            var text = $"сообщение от @id{msg.FromId} ({username}):\n\n{msg.GetParams()}";
            await VkApi.Messages.Send(DbHelper.GetAdmins(), text);

            Message = "Сообщение успешно отправлено!";
        }

        public bool CanExecute(Message msg)
        {
            if (string.IsNullOrEmpty(msg.GetParams()))
            {
                Message = "Введите сообщение для отправки.";
                return false;
            }

            return true;
        }
    }
}