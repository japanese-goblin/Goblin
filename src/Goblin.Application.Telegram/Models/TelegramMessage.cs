using Goblin.Application.Core.Abstractions;
using Telegram.Bot.Types;

namespace Goblin.Application.Telegram.Models
{
    public class TelegramMessage : Message, IMessage
    {
        public long FromUserId => Chat.Id;
        public long ToUserId => 0; // TODO: ?
        public string Payload { get; set; } = ""; //TODO: ?
    }
}