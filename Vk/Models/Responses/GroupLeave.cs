using Newtonsoft.Json;

namespace Vk.Models.Responses
{
    // {"type":"group_leave","object":{"user_id":***REMOVED***,"self":1},"group_id":146048760}
    public class GroupLeave
    {
        [JsonProperty("user_id")]
        public long UserId { get; set; }
        [JsonProperty("self")]
        public int Self { get; set; }

        public static GroupLeave FromJson(string str) => JsonConvert.DeserializeObject<GroupLeave>(str);
    }
}