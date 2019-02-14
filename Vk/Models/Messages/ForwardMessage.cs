using Newtonsoft.Json;

namespace Vk.Models.Messages
{
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