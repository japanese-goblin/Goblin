using Newtonsoft.Json;

namespace Vk.Models
{
    public class UploadServerInfo
    {
        [JsonProperty("upload_url")]
        public string UploadUrl { get; set; }
        [JsonProperty("album_id")]
        public int AlbumId { get; set; }
        [JsonProperty("group_id")]
        public int GroupId { get; set; }
    }
}