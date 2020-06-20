using System.Linq;

namespace Goblin.Application.Core.Abstractions
{
    public interface IMessage
    {
        public long MessageUserId { get; }
        public long MessageChatId { get; }
        public string Text { get; set; }
        public string Payload { get; }

        public string[] MessageParams => Text.ToLower().Split(' ').Skip(1).ToArray();
        public string CommandName => Text.ToLower().Split(' ').FirstOrDefault();
    }
}