﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Vk.Models;

namespace Vk
{
    public static class VkApi
    {
        private const string EndPoint = "https://api.vk.com/method";
        private static readonly WebClient Client = new WebClient();
        private const string Version = "5.92";
        internal static string AccessToken { get; set; }

        #region categories

        public static readonly Messages Messages = new Messages();
        public static readonly Users Users = new Users();

        #endregion

        public static void SetAccessToken(string token) => AccessToken = token;

        internal static async Task<string> SendRequest(string method, Dictionary<string, string> @params)
        {
            if(string.IsNullOrEmpty(AccessToken)) throw new Exception("Токен отсутствует");

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

            if (responseStr.Contains("error"))
            {
                var error = JsonConvert.DeserializeObject<Error>(responseStr);
                throw new Exception($"[{method}]: {error.Info.ErrorMsg}");
            }
            return responseStr;
        }
    }
}