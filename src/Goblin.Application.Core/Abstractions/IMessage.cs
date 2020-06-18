using System.Linq;

namespace Goblin.Application.Core.Abstractions
{
    public interface IMessage
    {
        public long FromUserId { get; }
        public long ToUserId { get; }
        public string Text { get; set; }
        public string Payload { get; set; }

        public string[] MessageParams => Text.ToLower().Split(' ').Skip(1).ToArray();
        public string CommandName => Text.ToLower().Split(' ').FirstOrDefault();
    }
}