using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Flurl;
using Flurl.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Vk.Category;
using Vk.Models;

namespace Vk
{
    public class VkApi
    {
        public const string EndPoint = "https://api.vk.com/method/";
        private const string Version = "5.92";
        private const string Language = "ru";
        private readonly string AccessToken;

        public VkApi(string token)
        {
            if(string.IsNullOrEmpty(token))
            {
                throw new ArgumentNullException(nameof(token), "Токен отсутствует");
            }

            AccessToken = token;
            
            Messages = new Messages(this); //TODO: нормальный DI
            Users = new Users(this);
            Photos = new Photos(this);
        }

        internal async Task<T> CallApi<T>(string method, Dictionary<string, string> @params)
        {
            //TODO add sleep? (лимит для токена сообщества - 20 запросов в секунду)
            var response = await BuildRequest()
                               .AppendPathSegment(method)
                               .SetQueryParams(@params)
                               .PostAsync(null);

            //if(!response.IsSuccessStatusCode) //TODO: не сработает, потому что вк всегда возвращает 200
            //{
            //    //TODO
            //    //var error = JsonConvert.DeserializeObject<Error>(responseStr);
            //    throw new Exception($"[{method}]: error");
            //}

            var x = JsonConvert.DeserializeObject<ApiResponse<T>>(await response.Content.ReadAsStringAsync());

            return x.Response;
        }

        internal IFlurlRequest BuildRequest()
        {
            return EndPoint.SetQueryParam("lang", Language)
                           .SetQueryParam("access_token", AccessToken)
                           .SetQueryParam("v", Version)
                           .WithHeaders(new
                           {
                               Accept = "application/json",
                               User_Agent = "Japanese Goblin 1.0"
                           })
                           .AllowAnyHttpStatus(); //TODO:
        }

        #region categories
        public readonly Messages Messages;
        public readonly Users Users;
        public readonly Photos Photos;
        #endregion
    }
}