using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Flurl;
using Flurl.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Vk.Category;
using Vk.Models;

namespace Vk
{
    public class VkApi
    {
        public const string EndPoint = "https://api.vk.com/method/";
        private const string Version = "5.92";
        private const string Language = "ru";

        private readonly string _token;
        private readonly ILogger _logger;

        public VkApi(string token, ILogger logger)
        {
            if(string.IsNullOrEmpty(token))
            {
                throw new ArgumentNullException(nameof(token), "Токен отсутствует");
            }

            _token = token;
            _logger = logger;

            Messages = new Messages(this); //TODO: нормальный DI
            Users = new Users(this);
            Photos = new Photos(this);
        }

        internal async Task<T> CallApi<T>(string method, Dictionary<string, string> @params)
        {
            using(_logger.BeginScope("Вызов метода {0}", method))
            {
                _logger.LogInformation("С параметрами {0}", @params);
            }
            //TODO add sleep? (лимит для токена сообщества - 20 запросов в секунду)
            var response = await BuildRequest()
                                 .AppendPathSegment(method)
                                 .PostUrlEncodedAsync(@params);

            //if(!response.IsSuccessStatusCode) //TODO: не сработает, потому что вк всегда возвращает 200
            //{
            //    //TODO
            //    //var error = JsonConvert.DeserializeObject<Error>(responseStr);
            //    throw new Exception($"[{method}]: error");
            //}

            var result = JsonConvert.DeserializeObject<ApiResponse<T>>(await response.Content.ReadAsStringAsync());

            return result.Response;
        }

        internal IFlurlRequest BuildRequest()
        {
            return EndPoint.SetQueryParam("lang", Language)
                           .SetQueryParam("access_token", _token)
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