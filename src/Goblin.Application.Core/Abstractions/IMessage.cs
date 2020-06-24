using System.Linq;

namespace Goblin.Application.Core.Abstractions
{
    public interface IMessage
    {
        public long MessageUserId { get; }
        public long MessageChatId { get; }
        public string Text { get; }
        public string Payload { get; }

        public string[] MessageParams
        {
            get
            {
                var data = Text.ToLower().Split(' ');
                return data.Length <= 1 ? new[] { string.Empty } : data.Skip(1).ToArray();
            }
        }

        public string CommandName => Text.ToLower().Split(' ').FirstOrDefault();
    }
}