using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Goblin.Helpers
{
    public static class VkHelper
    {
        //***REMOVED***
        public const string ConfirmationToken = "***REMOVED***";

        //***REMOVED***
        private const string VkToken = "***REMOVED***";

        public static List<int> DevelopersID = new List<int> {***REMOVED***};

        public static async Task<bool> SendMessage(int id, string text, string attach = "")
        {
            return await SendMessage(new List<int> {id}, text, attach);
        }

        public static async Task<bool> SendMessage(List<int> ids, string text, string attach = "")
        {
            if (string.IsNullOrEmpty(text)) return false;
            using (var client = new WebClient())
            {
                var values = new NameValueCollection
                {
                    ["message"] = text,
                    ["access_token"] = VkToken,
                    ["v"] = "5.80",
                    ["attachment"] = attach
                };

                if (ids.Count > 1)
                {
                    values.Add("user_ids", string.Join(",", ids));
                }
                else
                {
                    values.Add("peer_id", ids[0].ToString());
                }

                var response = await client.UploadValuesTaskAsync(new Uri("https://api.vk.com/method/messages.send"), values);

                var responseString = JsonConvert.DeserializeObject<dynamic>(Encoding.Default.GetString(response));
                return int.TryParse(responseString["response"]?.ToString(), out int result); // TODO: ???
            }
        }

        public static async Task<string> GetUserName(int id)
        {
            //https://api.vk.com/method/users.get?user_ids={$user_id}&v=5.0&lang=ru
            using (var client = new WebClient())
            {
                var values = new NameValueCollection
                {
                    ["user_ids"] = id.ToString(),
                    ["v"] = "5.80",
                    ["lang"] = "ru",
                    ["access_token"] = VkToken
                };
                var response = await client.UploadValuesTaskAsync("https://api.vk.com/method/users.get", values);
                var responseString = JsonConvert.DeserializeObject<dynamic>(Encoding.Default.GetString(response));
                var result = responseString["response"];
                if (result.ToString() == "[]")
                    return string.Empty;
                var name = $"{result[0]["first_name"]} {result[0]["last_name"]}";
                return name;
            }
        }
    }
}