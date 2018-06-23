using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.Text;
using Goblin.Models;
using Newtonsoft.Json;

namespace Goblin
{
    public static class Utils
    {
        //***REMOVED***
        public const string ConfirmationToken = "***REMOVED***";
        //***REMOVED***
        private const string VkToken = "***REMOVED***";
        public static MainContext DB { get; set; }

        public static List<int> DevelopersID = new List<int>() { ***REMOVED*** };

        public static bool SendMessage(int id, string text, string attach = "", string kb = "")
        {
            if (string.IsNullOrEmpty(text)) return false;
            using (var client = new WebClient())
            {
                var values = new NameValueCollection
                {
                    ["user_id"] = id.ToString(),
                    ["message"] = text,
                    ["v"] = "5.80",
                    ["access_token"] = VkToken,
                };
                if (!string.IsNullOrEmpty(attach))
                    values.Add("attachment", attach);
                if (!string.IsNullOrEmpty(kb))
                    values.Add("keyboard", kb);

                var response = client.UploadValues("https://api.vk.com/method/messages.send", values);

                var responseString = JsonConvert.DeserializeObject<dynamic>(Encoding.Default.GetString(response));
                return int.TryParse(responseString["response"].ToString(), out int result); // TODO: ???
            }
        }

        public static bool SendMessage(List<int> ids, string text, string attach = "")
        {
            if (string.IsNullOrEmpty(text)) return false;
            using (var client = new WebClient())
            {
                var values = new NameValueCollection
                {
                    ["message"] = text,
                    ["user_ids"] = string.Join(",", ids),
                    ["access_token"] = VkToken,
                    ["v"] = "5.0",
                    ["attachment"] = attach
                };

                var response = client.UploadValues("https://api.vk.com/method/messages.send", values);

                var responseString = JsonConvert.DeserializeObject<dynamic>(Encoding.Default.GetString(response));
                return int.TryParse(responseString["response"][0].ToString(), out int result); // TODO: ???
            }
        }

        public static string GetUserName(int id)
        {
            //https://api.vk.com/method/users.get?user_ids={$user_id}&v=5.0&lang=ru
            using (var client = new WebClient())
            {
                var values = new NameValueCollection
                {
                    ["user_ids"] = id.ToString(),
                    ["v"] = "5.0",
                    ["lang"] = "ru"
                };
                var response = client.UploadValues("https://api.vk.com/method/users.get", values);
                var responseString = JsonConvert.DeserializeObject<dynamic>(Encoding.Default.GetString(response));
                var name = $"{responseString["response"][0]["first_name"]} {responseString["response"][0]["last_name"]}";
                return name;
            }
        }

        public static string CreateMD5(string input)
        {
            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                var inputBytes = Encoding.ASCII.GetBytes(input);
                var hashBytes = md5.ComputeHash(inputBytes);

                var sb = new StringBuilder();
                foreach (var t in hashBytes)
                {
                    sb.Append(t.ToString("X2"));
                }
                return sb.ToString().ToLower();
            }
        }
    }
}