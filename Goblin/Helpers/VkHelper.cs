using Goblin.Models.Keyboard;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Goblin.Helpers
{
    public static class VkHelper
    {
        public static List<long> DevelopersID = new List<long> { ***REMOVED*** };

        #region vk params
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
        /// <param name="attach">прикрепления</param>
        /// <param name="kb">клавиатура</param>
        /// <returns></returns>
        public static async Task SendMessage(long id, string text, string attach = "", Keyboard kb = null)
        {
            await SendMessage(new List<long> { id }, text, attach, kb);
        }

        /// <summary>
        /// Сообщение для нескольких пользователей
        /// </summary>
        /// <param name="ids">ID пользователя в вк</param>
        /// <param name="text">сообщение</param>
        /// <param name="attach">прикрепления</param>
        /// <param name="kb">клавиатура</param>
        /// <returns></returns>
        public static async Task SendMessage(List<long> ids, string text, string attach = "", Keyboard kb = null)
        {
            if (string.IsNullOrEmpty(text)) return;
            using (var client = new WebClient())
            {
                var values = new NameValueCollection
                {
                    ["message"] = text,
                    ["access_token"] = VkToken,
                    ["v"] = "5.80",
                    ["attachment"] = attach
                };

                values.Add("user_ids", string.Join(",", ids));

                var isKb = kb is null;
                if (!isKb)
                {
                    values.Add("keyboard", kb.ToString());
                }

                await client.UploadValuesTaskAsync("https://api.vk.com/method/messages.send", values);
            }
        }

        /// <summary>
        /// Получение фамилии и имени пользователя по ID
        /// </summary>
        /// <param name="id">ID пользователя в вк</param>
        /// <returns>строка с фамилией и именем</returns>
        public static async Task<string> GetUserName(long id)
        {
            using (var client = new WebClient())
            {
                var values = new NameValueCollection
                {
                    ["user_ids"] = id.ToString(),
                    ["v"] = "5.80",
                    ["lang"] = "ru",
                    ["access_token"] = VkToken
                };
                var response = await client.UploadValuesTaskAsync("https://api.vk.com/method/users.get", values);
                var responseString = JsonConvert.DeserializeObject<dynamic>(Encoding.Default.GetString(response));
                var result = responseString["response"];
                if (result.ToString() == "[]")
                    return string.Empty;
                var name = $"{result[0]["first_name"]} {result[0]["last_name"]}";
                return name;
            }
        }
    }

}