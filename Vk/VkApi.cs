using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Vk.Category;

namespace Vk
{
    public class VkApi
    {
        private const string EndPoint = "https://api.vk.com/method";
        private readonly HttpClient Client = new HttpClient(); //TODO: DI
        private const string Version = "5.92";
        private string AccessToken { get; set; }

        public VkApi(string token)
        {
            AccessToken = token;
            Messages = new Messages(this);
            Users = new Users(this);
            Photos = new Photos(this);
        }

        internal async Task<string> CallApi(string method, Dictionary<string, string> @params)
        {
            if(string.IsNullOrEmpty(AccessToken)) throw new Exception("Токен отсутствует");

            var reqParams = new Dictionary<string, string>();
            foreach(var (param, value) in @params)
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

            if(responseStr.Contains("error"))
            {
                //TODO
                //var error = JsonConvert.DeserializeObject<Error>(responseStr);
                throw new Exception($"[{method}]: error");
            }

            string items = Convert.ToString(JsonConvert.DeserializeObject<dynamic>(responseStr)["response"]);
            return items;
        }

        #region categories

        public readonly Messages Messages;
        public readonly Users Users;
        public readonly Photos Photos;

        #endregion
    }
}
