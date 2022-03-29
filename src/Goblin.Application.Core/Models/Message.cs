using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Goblin.Application.Core.Models
{
    public class Message
    {
        public long UserId { get; set; }
        public long ChatId { get; set; }

        public string UserTag { get; set; }
        
        public string Text { get; set; }
        public string Payload { get; set; }

        public bool IsConversation => UserId != ChatId;

        public string[] CommandParameters
        {
            get
            {
                var data = Text.ToLower().Split(' ');
                return data.Length <= 1 ? new[] { string.Empty } : data[1..];
            }
        }

        public string CommandName => Text.ToLower().Split(' ').FirstOrDefault();

        public Dictionary<string, string> ParsedPayload => JsonConvert.DeserializeObject<Dictionary<string, string>>(Payload);
    }
}