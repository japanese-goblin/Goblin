using Newtonsoft.Json;

namespace Vk.Models
{
    public class User
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("first_name")]
        public string FirstName { get; set; }
        [JsonProperty("last_name")]
        public string LastName { get; set; }
        [JsonProperty("is_closed")]
        public bool? IsClosed { get; set; }
        [JsonProperty("can_access_closed")]
        public bool? CanAccessClosed { get; set; }
        [JsonProperty("deactivated")]
        public string Deactivated { get; set; }
        [JsonProperty("photo_200_orig")]
        public string Photo200Orig { get; set; }

        public override string ToString() => $"{FirstName} {LastName}";
    }
}