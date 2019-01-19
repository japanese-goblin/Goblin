using Newtonsoft.Json;

namespace Vk.Models
{
    public class UploadImageInfo
    {
        [JsonProperty("server")] public int Server { get; set; }
        [JsonProperty("photo")] public string Photo { get; set; }
        [JsonProperty("hash")] public string Hash { get; set; }
    }
}