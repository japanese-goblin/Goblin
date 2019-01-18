using System.Threading.Tasks;
using Goblin.Helpers;
using Vk;
using Vk.Models.Messages;

namespace Goblin.Bot.Commands
{
    public class SendAdmin : ICommand
    {
        public string Name { get; } = "Админ *сообщение*";
        public string Decription { get; } = "Отправляет сообщение администраторам бота.";
        public string Usage { get; } = "Админ хелп";
        public string[] Allias { get; } = {"админ", "адм"};
        public Category Category { get; } = Category.Common;
        public bool IsAdmin { get; } = false;

        public async Task<CommandResponse> Execute(Message msg)
        {
            var username = await VkApi.Users.GetUserName(msg.FromId);
            var text = $"сообщение от @id{msg.FromId} ({username}):\n\n{msg.GetParams()}";
            await VkApi.Messages.Send(DbHelper.GetAdmins(), text);

            return new CommandResponse
            {
                Text = "Ваше сообщение успешно отправлено администраторам"
            };
        }

        public (bool Success, string Text) CanExecute(Message msg)
        {
            if (string.IsNullOrEmpty(msg.GetParams()))
            {
                return (false, "Ошибка. Введите сообщение для отправки.");
            }

            return (true, "");
        }
    }
}