using System.Threading.Tasks;
using Goblin.Bot.Enums;
using Goblin.Bot.Models;
using Goblin.Persistence;
using Vk;
using Vk.Models.Messages;

namespace Goblin.Bot.Commands
{
    public class SendAdmin : ICommand
    {
        public string Name { get; } = "Админ *сообщение*";
        public string Decription { get; } = "Отправляет сообщение администраторам бота.";
        public string Usage { get; } = "Админ хелп";
        public string[] Allias { get; } = { "админ", "адм" };
        public CommandCategory Category { get; } = CommandCategory.Common;
        public bool IsAdmin { get; } = false;

        private readonly ApplicationDbContext _db;
        private readonly VkApi _api;

        public SendAdmin(ApplicationDbContext db, VkApi api)
        {
            _db = db;
            _api = api;
        }

        public async Task<CommandResponse> Execute(Message msg)
        {
            var username = await _api.Users.Get(msg.FromId);
            var text = $"сообщение от @id{msg.FromId} ({username}):\n\n{msg.GetParams()}";
            await _api.Messages.Send(_db.GetAdmins(), text);

            return new CommandResponse
            {
                Text = "Ваше сообщение успешно отправлено администраторам"
            };
        }

        public (bool Success, string Text) CanExecute(Message msg)
        {
            if(string.IsNullOrEmpty(msg.GetParams()))
            {
                return (false, "Ошибка. Введите сообщение для отправки.");
            }

            return (true, "");
        }
    }
}