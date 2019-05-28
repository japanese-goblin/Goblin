using Newtonsoft.Json;

namespace Vk.Models.Responses
{
    // {"type":"group_join","object":{"user_id":366305213,"join_type":"join"},"group_id":146048760}
    public class GroupJoin
    {
        [JsonProperty("user_id")]
        public long UserId { get; set; }

        [JsonProperty("join_type")]
        public string JoinType { get; set; }

        public static GroupJoin FromJson(string str)
        {
            return JsonConvert.DeserializeObject<GroupJoin>(str);
        }
    }
}