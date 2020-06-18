using Goblin.Application.Core.Abstractions;
using Telegram.Bot.Types;

namespace Goblin.Application.Telegram.Models
{
    public class TelegramMessage : Message, IMessage
    {
        public long MessageUserId => From.Id;
        public long MessageChatId => Chat.Id;
        public string Payload { get; set; } = ""; //TODO: ?
    }
}