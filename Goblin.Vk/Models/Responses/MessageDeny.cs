using Newtonsoft.Json;

namespace Goblin.Vk.Models.Responses
{
    // {"type":"message_deny","object":{"user_id":***REMOVED***},"group_id":146048760}
    public class MessageDeny
    {
        [JsonProperty("user_id")]
        public long UserId { get; set; }

        public static MessageDeny FromJson(string str) => JsonConvert.DeserializeObject<MessageDeny>(str);
    }
}