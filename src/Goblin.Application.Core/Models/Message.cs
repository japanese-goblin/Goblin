using System.Linq;

namespace Goblin.Application.Core.Models
{
    public class Message
    {
        public long UserId { get; set; }
        public long ChatId { get; set; }
        public string Text { get; set; }
        public string Payload { get; set; }

        public string[] CommandParameters
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