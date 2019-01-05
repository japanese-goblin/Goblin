using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Vk
{
    public static class Api
    {
        private const string EndPoint = "https://api.vk.com/method/";
        private static readonly WebClient Client = new WebClient();
        private const string Version = "5.92";
        internal static string AccessToken { get; set; }

        public static void SetAccessToken(string token) => AccessToken = token;

        internal static async Task<string> SendRequest(string method, Dictionary<string, string> @params)
        {
            //TODO add sleep?
            var reqParams = new NameValueCollection();
            foreach (var (param, value) in @params)
            {
                reqParams.Add(param, value);
            }
            reqParams.Add("v", Version);
            reqParams.Add("access_token", AccessToken);

            var response = await Client.UploadValuesTaskAsync($"{EndPoint}/{method}", reqParams);
            var responseStr = Encoding.Default.GetString(response);

            //if (responseStr.Contains("error"))
            //{
            //    // TODO: logger?
            //}
            return responseStr;
        }
    }
}