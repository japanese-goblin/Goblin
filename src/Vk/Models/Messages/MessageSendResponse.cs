using Newtonsoft.Json;

namespace Vk.Models.Messages
{
    public class MessageSendResponse
    {
        [JsonProperty("peer_id")]
        public long PeerId { get; set; }

        [JsonProperty("message_id")]
        public int MessageId { get; set; }
    }
}