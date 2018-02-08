﻿using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.Text;
using Newtonsoft.Json;

namespace Goblin
{
    public static class Utils
    {
        public const string ConfirmationToken = "***REMOVED***";
        private const string VkToken = "***REMOVED***";

        public static List<int> DevelopersID = new List<int>() { ***REMOVED*** };

        public static bool SendMessage(int id, string text, string attach = "")
        {
            using (var client = new WebClient())
            {
                var values = new NameValueCollection
                {
                    ["message"] = text,
                    ["user_id"] = id.ToString(),
                    ["access_token"] = VkToken,
                    ["v"] = "5.0",
                    ["attachment"] = attach
                };

                var response = client.UploadValues("https://api.vk.com/method/messages.send", values);

                var responseString = JsonConvert.DeserializeObject<dynamic>(Encoding.Default.GetString(response));
                return int.TryParse(responseString["response"].ToString(), out int result); // TODO: ???
            }
        }
    }
}