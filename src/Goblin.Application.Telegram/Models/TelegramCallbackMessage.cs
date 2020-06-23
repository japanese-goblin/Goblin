using Goblin.Application.Core.Abstractions;
using Telegram.Bot.Types;

namespace Goblin.Application.Telegram.Models
{
    public class TelegramCallbackMessage : CallbackQuery, IMessage
    {
        public long MessageUserId => From.Id;
        public long MessageChatId => From.Id;
        public string Text => Message.Text;
        public string Payload => Data;
    }
}