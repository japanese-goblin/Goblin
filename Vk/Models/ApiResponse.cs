using Newtonsoft.Json;

namespace Vk.Models
{
    public class ApiResponse<T>
    {
        [JsonProperty("Response")]
        public T Response { get; set; }
    }
}