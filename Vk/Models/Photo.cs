using Newtonsoft.Json;

namespace Vk.Models
{
    public class Photo
    {
        [JsonProperty("id")]
        public uint Id { get; set; }

        [JsonProperty("album_id")]
        public int AlbumId { get; set; }

        [JsonProperty("owner_id")]
        public int OwnerId { get; set; }

        [JsonProperty("sizes")]
        public Size[] Sizes { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("date")]
        public uint Date { get; set; }

        [JsonProperty("access_key")]
        public string AccessKey { get; set; }
    }

    public class Size
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("width")]
        public uint Width { get; set; }

        [JsonProperty("height")]
        public uint Height { get; set; }
    }
}
