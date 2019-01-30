using Newtonsoft.Json;

namespace Vk.Models
{
    //TODO эта модель не ко всему подходит, чудеса просто
    public class Error
    {
        [JsonProperty("error")]
        public ErrorInfo Info { get; set; }
    }

    public class ErrorInfo
    {
        [JsonProperty("error_code")]
        public int ErrorCode { get; set; }

        [JsonProperty("error_msg")]
        public string ErrorMsg { get; set; }

        [JsonProperty("request_params")]
        public RequestParams[] RequestParams { get; set; }
    }

    public class RequestParams
    {
        [JsonProperty("key")]
        public string Key { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }
    }
}
