using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vk.Models;

namespace Vk
{
    public class Users
    {
        public async Task<string[]> GetUserName(long[] ids)
        {
            var values = new Dictionary<string, string>
            {
                ["user_ids"] = string.Join(',', ids),
                ["lang"] = "ru"
            };
            var response = await VkApi.SendRequest("users.get", values);
            var usersInfo = JsonConvert.DeserializeObject<UsersGetReponse>(response);
            if (usersInfo.Response.Length == 0)
            {
                return new string[] { }; // TODO ?
            }

            return usersInfo.Response.Select(x => x.ToString()).ToArray();
        }
        public async Task<string> GetUserName(long id)
        {
            return (await GetUserName(new[] { id })).FirstOrDefault(); //TODO ?
        }
    }
}