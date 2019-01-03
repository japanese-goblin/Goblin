using Newtonsoft.Json;

namespace Goblin.Vk.Models.Responses
{
    public class MessageNew
    {
        [JsonProperty("date")]
        public int Date { get; set; }
        [JsonProperty("from_id")]
        public long FromId { get; set; }
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("out")]
        public bool Out { get; set; }
        [JsonProperty("peer_id")]
        public long PeerId { get; set; }
        [JsonProperty("text")]
        public string Text { get; set; }
        [JsonProperty("conversation_message_id")]
        public int ConversationMessageId { get; set; }
        [JsonProperty("fwd_messages")]
        public ForwardMessage[] ForwardMessages { get; set; }
        [JsonProperty("important")]
        public bool Important { get; set; }
        [JsonProperty("random_id")]
        public long RandomId { get; set; }
        [JsonProperty("attachments")]
        public object[] Attachments { get; set; }
        [JsonProperty("is_hidden")]
        public bool IsHidden { get; set; }

        public static MessageNew FromJson(string str) => JsonConvert.DeserializeObject<MessageNew>(str);
    }

    public class ForwardMessage
    {
        [JsonProperty("date")]
        public int Date { get; set; }
        [JsonProperty("from_id")]
        public int FromId { get; set; }
        [JsonProperty("text")]
        public string Text { get; set; }
        [JsonProperty("attachments")]
        public object[] Attachments { get; set; }
        [JsonProperty("update_time")]
        public int UpdateTime { get; set; }
    }
}