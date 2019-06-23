using System.Threading.Tasks;
using Goblin.Bot.Enums;
using Goblin.Bot.Models;
using Goblin.Domain.Entities;
using Goblin.Persistence;
using Vk;
using Vk.Models.Messages;

namespace Goblin.Bot.Commands
{
    public class SendAdmin : ICommand
    {
        public string Name { get; } = "Админ *сообщение*";
        public string Description { get; } = "Отправляет сообщение администраторам.";
        public string Usage { get; } = "Админ хелп";
        public string[] Aliases { get; } = { "админ", "адм" };
        public CommandCategory Category { get; } = CommandCategory.Common;
        public bool IsAdmin { get; } = false;

        private readonly BotDbContext _db;
        private readonly VkApi _api;

        public SendAdmin(BotDbContext db, VkApi api)
        {
            _db = db;
            _api = api;
        }

        public async Task<CommandResponse> Execute(Message msg, BotUser user)
        {
            var username = await _api.Users.Get(msg.FromId);
            var text = $"сообщение от @id{msg.FromId} ({username}):\n\n{msg.GetParams()}";
            await _api.Messages.Send(_db.GetAdmins(), text);

            return new CommandResponse
            {
                Text = "Ваше сообщение успешно отправлено администраторам"
            };
        }

        public (bool Success, string Text) CanExecute(Message msg, BotUser user)
        {
            if(string.IsNullOrEmpty(msg.GetParams()))
            {
                return (false, "Ошибка. Введите сообщение для отправки.");
            }

            return (true, "");
        }
    }
}