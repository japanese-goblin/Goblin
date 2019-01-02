using Goblin.Vk.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Goblin.Vk
{
    public static class VkMethods
    {
        #region settings

        private const string EndPoint = "https://api.vk.com/method/";
        private static readonly WebClient Client = new WebClient();
        private const string Version = "5.92";

        public static long[] DevelopersID = new[] { (long)***REMOVED*** }; // TODO: вынести в бд?

        //***REMOVED***
        public const string ConfirmationToken = "***REMOVED***";

        //***REMOVED***
        private const string VkToken = "***REMOVED***";
        #endregion

        /// <summary>
        /// Сообщение для одного пользователя
        /// </summary>
        /// <param name="id">ID пользователя в вк</param>
        /// <param name="text">сообщение</param>
        /// <param name="attachs">прикрепления</param>
        /// <param name="kb">клавиатура</param>
        /// <returns></returns>
        public static async Task SendMessage(long id, string text, string[] attachs = null, Keyboard kb = null)
        {
            await SendMessage(new[] { id }, text, attachs, kb);
        }

        /// <summary>
        /// Сообщение для нескольких пользователей
        /// </summary>
        /// <param name="ids">ID пользователя в вк</param>
        /// <param name="text">сообщение</param>
        /// <param name="attachs">прикрепления</param>
        /// <param name="kb">клавиатура</param>
        /// <returns></returns>
        public static async Task SendMessage(long[] ids, string text, string[] attachs = null, Keyboard kb = null)
        {
            if (string.IsNullOrEmpty(text) || ids.Length == 0)
            {
                return;
            }

            var values = new Dictionary<string, string>()
            {
                ["message"] = text,
                ["access_token"] = VkToken,
                ["random_id"] = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString(),
                ["peer_ids"] = string.Join(',', ids)
            };

            if (!(attachs is null)) // если есть аттачи
            {
                values.Add("attachment", string.Join(",", attachs));
            }

            //if (ids.Count > 1)
            //{
            //    values.Add();
            //}
            //else
            //{
            //    values.Add("peer_id", ids[0].ToString());
            //}

            if (!(kb is null))
            {
                values.Add("keyboard", kb.ToString());
            }

            await SendRequest("messages.send", values); //TODO to bool?
        }

        /// <summary>
        /// Получение фамилии и имени пользователя по ID
        /// </summary>
        /// <param name="id">ID пользователя</param>
        /// <returns>строка с фамилией и именем</returns>
        public static async Task<string> GetUserName(long id)
        {
            var values = new Dictionary<string, string>
            {
                ["user_ids"] = id.ToString(),
                ["lang"] = "ru",
                ["access_token"] = VkToken
            };
            var response = await SendRequest("users.get", values);
            var usersInfo = JsonConvert.DeserializeObject<UsersGetReponse>(response);

            return usersInfo.Response[0].ToString();
        }

        private static async Task<string> SendRequest(string method, Dictionary<string, string> @params)
        {
            //TODO add sleep?
            var reqParams = new NameValueCollection();
            foreach (var param in @params)
            {
                reqParams.Add(param.Key, param.Value);
            }
            reqParams.Add("v", Version);

            var response = await Client.UploadValuesTaskAsync($"{EndPoint}/{method}", reqParams);
            var responseStr = Encoding.Default.GetString(response);

            //if (responseStr.Contains("error"))
            //{
            //    // TODO: logger?
            //}
            return responseStr;
        }
    }

    //public static async Task SendToConversation(long id, int group, string city = "")
    //{
    //    var schedule = await ScheduleHelper.GetScheduleAtDate(DateTime.Now, group);
    //    await SendMessage(id, schedule);

    //    if (!string.IsNullOrEmpty(city))
    //    {
    //        var weather = await WeatherHelper.GetWeather(city);
    //        await SendMessage(id, weather);
    //    }
    //}
}