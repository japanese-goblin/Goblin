using Newtonsoft.Json;

namespace Goblin.Vk.Models.Responses
{
    // {"type":"group_join","object":{"user_id":***REMOVED***,"join_type":"join"},"group_id":146048760}
    public class GroupJoin
    {
        [JsonProperty("user_id")]
        public long UserId { get; set; }
        [JsonProperty("join_type")]
        public string JoinType { get; set; }

        public static GroupJoin FromJson(string str) => JsonConvert.DeserializeObject<GroupJoin>(str);
    }
}