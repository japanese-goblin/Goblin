using System.Linq;

namespace Goblin.Application.Core.Abstractions
{
    public interface IMessage
    {
        public long MessageUserId { get; }
        public long MessageChatId { get; }
        public string MessageText { get; }
        public string MessagePayload { get; }

        public string[] MessageParams
        {
            get
            {
                var data = MessageText.ToLower().Split(' ');
                return data.Length <= 1 ? new[] { string.Empty } : data.Skip(1).ToArray();
            }
        }

        public string CommandName => MessageText.ToLower().Split(' ').FirstOrDefault();
    }
}