using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
namespace Vk
{
    public static class VkApi
    {
        private const string EndPoint = "https://api.vk.com/method";
        private static readonly HttpClient Client = new HttpClient();
        private const string Version = "5.92";
        internal static string AccessToken { get; set; }

        public static void SetAccessToken(string token) => AccessToken = token;

        internal static async Task<string> SendRequest(string method, Dictionary<string, string> @params)
        {
            if(string.IsNullOrEmpty(AccessToken)) throw new Exception("Токен отсутствует");

            var reqParams = new Dictionary<string, string>();
            foreach (var (param, value) in @params)
            {
                reqParams.Add(param, value);
            }
            reqParams.Add("lang", "ru");
            reqParams.Add("v", Version);
            reqParams.Add("access_token", AccessToken);

            //TODO add sleep?
            var response = await Client.PostAsync($"{EndPoint}/{method}",
                new FormUrlEncodedContent(reqParams));
            var responseStr = await response.Content.ReadAsStringAsync();

            if (responseStr.Contains("error"))
            {
                //TODO
                //var error = JsonConvert.DeserializeObject<Error>(responseStr);
                throw new Exception($"[{method}]: error");
            }

            return responseStr;
        }

        #region categories

        public static readonly Messages Messages = new Messages();
        public static readonly Users Users = new Users();

        #endregion
    }
}