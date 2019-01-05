using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Vk.Models;

namespace Vk
{
    public static class Users
    {
        public static async Task<string[]> GetUserName(long[] ids)
        {
            var values = new Dictionary<string, string>
            {
                ["user_ids"] = string.Join(',', ids),
                ["lang"] = "ru"
            };
            var response = await Api.SendRequest("users.get", values);
            var usersInfo = JsonConvert.DeserializeObject<UsersGetReponse>(response);
            if (usersInfo.Response.Count == 0)
            {
                return new string[] { }; // TODO ?
            }

            return usersInfo.Response.Select(x => x.ToString()).ToArray();
        }
        public static async Task<string> GetUserName(long id)
        {
            return (await GetUserName(new[] {id})).FirstOrDefault(); //TODO ?
        }
    }
}