using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Vk.Models;

namespace Vk.Categories
{
    public class Users
    {
        public async Task<User> Get(long id)
        {
            return (await Get(new[] { id })).FirstOrDefault(); //TODO ?
        }

        public async Task<User[]> Get(long[] ids)
        {
            var values = new Dictionary<string, string>
            {
                ["user_ids"] = string.Join(',', ids),
                ["fields"] = "photo_200_orig"
            };

            var response = await VkApi.SendRequest("users.get", values);
            var usersInfo = JsonConvert.DeserializeObject<User[]>(response);

            if (usersInfo.Length == 0)
            {
                return new User[] { }; // TODO ?
            }

            return usersInfo.ToArray();
        }
    }
}