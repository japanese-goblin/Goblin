using System.Linq;
using Newtonsoft.Json;

namespace Vk.Models.Messages
{
    public class Message
    {
        [JsonProperty("date")]
        public uint Date { get; set; }

        [JsonProperty("from_id")]
        public long FromId { get; set; }

        [JsonProperty("id")]
        public uint Id { get; set; }

        [JsonProperty("out")]
        public bool Out { get; set; }

        [JsonProperty("peer_id")]
        public long PeerId { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("conversation_message_id")]
        public uint ConversationMessageId { get; set; }

        [JsonProperty("fwd_messages")]
        public ForwardMessage[] ForwardMessages { get; set; }

        [JsonProperty("reply_message")]
        public ForwardMessage[] ReplyMessages { get; set; }

        [JsonProperty("important")]
        public bool Important { get; set; }

        [JsonProperty("random_id")]
        public ulong RandomId { get; set; }

        [JsonProperty("attachments")]
        public object[] Attachments { get; set; } //TODO

        [JsonProperty("is_hidden")]
        public bool IsHidden { get; set; }

        public static Message FromJson(string str)
        {
            return JsonConvert.DeserializeObject<Message>(str);
        }

        public string GetParams()
        {
            var split = Text.Split(' ', 2);
            var param = split.Length > 1 ? split[1] : "";

            return param;
        }

        public string[] GetParamsAsArray()
        {
            var split = Text.Split(' ', 2);
            var param = split.Length > 1 ? split.Skip(1).ToArray() : new[] { "" };

            return param;
        }
    }
}