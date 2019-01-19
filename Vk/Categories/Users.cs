using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Vk.Models;

namespace Vk.Categories
{
    public class Users
    {
        public async Task<string> GetUserName(long id)
        {
            return (await GetUserName(new[] { id })).FirstOrDefault(); //TODO ?
        }

        public async Task<string[]> GetUserName(long[] ids)
        {
            var values = new Dictionary<string, string>
            {
                ["user_ids"] = string.Join(',', ids)
            };

            var response = await VkApi.SendRequest("users.get", values);
            var usersInfo = JsonConvert.DeserializeObject<User[]>(response);

            if (usersInfo.Length == 0)
            {
                return new string[] { }; // TODO ?
            }

            return usersInfo.Select(x => x.ToString()).ToArray();
        }
    }
}