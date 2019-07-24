using Newtonsoft.Json;

namespace Vk.Models
{
    public class CallbackResponse
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("object")]
        public object Object { get; set; }

        [JsonProperty("group_id")]
        public uint GroupId { get; set; }

        [JsonProperty("secret")]
        public string Secret { get; set; }
    }
}