namespace Goblin.Application.Core.Tests.Models
{
    public class Message
    {
        public long UserId { get; set; }
        public long ChatId { get; set; }
        public string Text { get; set; }

        public string Data { get; set; }
    }
}