using System.Collections.Specialized;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using Newtonsoft.Json;

namespace Goblin
{
    public static class Utils
    {
        public static string ConfirmationToken = "***REMOVED***";
        private static string VkToken = "***REMOVED***";

        public static bool SendMessage(int id, string text, string attach = "")
        {
            using (var client = new WebClient())
            {
                var values = new NameValueCollection();
                values["message"] = text;
                values["user_id"] = id.ToString();
                values["access_token"] = VkToken;
                values["v"] = "5.0";
                values["attachment"] = attach;

                var response = client.UploadValues("https://api.vk.com/method/messages.send", values);

                var responseString = JsonConvert.DeserializeObject<dynamic>(Encoding.Default.GetString(response));
                return int.TryParse(responseString["response"].ToString(), out int result);
                //
            }
        }
    }
}