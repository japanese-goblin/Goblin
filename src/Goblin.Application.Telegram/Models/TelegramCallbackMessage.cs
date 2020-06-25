using Goblin.Application.Core.Abstractions;
using Telegram.Bot.Types;

namespace Goblin.Application.Telegram.Models
{
    public class TelegramCallbackMessage : CallbackQuery, IMessage
    {
        public long MessageUserId => From.Id;
        public long MessageChatId => From.Id;
        public string MessageText => Message.Text;
        public string MessagePayload => Data;
    }
}