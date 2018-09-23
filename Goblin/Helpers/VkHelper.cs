using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VkNet;
using VkNet.Enums.Filters;
using VkNet.Model;
using VkNet.Model.Keyboard;
using VkNet.Model.RequestParams;

namespace Goblin.Helpers
{
    public static class VkHelper
    {
        public static List<int> DevelopersID = new List<int> {***REMOVED***};

        #region vk params
        //***REMOVED***
        public const string ConfirmationToken = "***REMOVED***";
        //***REMOVED***
        private const string VkToken = "***REMOVED***";
        private static VkApi api;
        static VkHelper()
        {
            api = new VkApi();
            api.Authorize(new ApiAuthParams
            {
                AccessToken = VkToken
            });
        }
        #endregion

        public static async Task SendMessage(int id, string text, MessageKeyboard kb = null)
        {
            await SendMessage(new List<int> {id}, text, kb);
        }

        public static async Task SendMessage(List<int> ids, string text, MessageKeyboard kb = null)
        {
            if (string.IsNullOrEmpty(text)) return;
            var param = new MessagesSendParams();
            if (ids.Count > 1)
            {
                param.UserIds = ids.Select(Convert.ToInt64);
            }
            else
            {
                param.PeerId = ids[0];
            }

            param.Message = text;
            var iskb = kb is null;
            if (!iskb)
            {
                param.Keyboard = kb;
            }

            await api.Messages.SendAsync(param);
        }

        public static async Task<string> GetUserName(int id)
        {
            var res = (await api.Users.GetAsync(new List<long>() {id}, ProfileFields.About)).First();
            return $"{res.FirstName} {res.LastName}";
        }
    }
}