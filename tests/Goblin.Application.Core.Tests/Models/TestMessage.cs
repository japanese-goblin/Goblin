using Goblin.Application.Core.Abstractions;

namespace Goblin.Application.Core.Tests.Models
{
    public class TestMessage : Message, IMessage
    {
        public long MessageUserId => UserId;
        public long MessageChatId => ChatId;
        public string Text => MessageText;
        public string Payload => Data;
    }
}