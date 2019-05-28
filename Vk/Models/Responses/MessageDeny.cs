using Newtonsoft.Json;

namespace Vk.Models.Responses
{
    // {"type":"message_deny","object":{"user_id":366305213},"group_id":146048760}
    public class MessageDeny
    {
        [JsonProperty("user_id")]
        public long UserId { get; set; }

        public static MessageDeny FromJson(string str)
        {
            return JsonConvert.DeserializeObject<MessageDeny>(str);
        }
    }
}