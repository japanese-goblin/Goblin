using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Vk.Models;

namespace Vk.Category
{
    public class Users
    {
        private readonly VkApi _api;

        public Users(VkApi api)
        {
            _api = api;
        }

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

            var response = await _api.CallApi("users.get", values);
            var usersInfo = JsonConvert.DeserializeObject<User[]>(response);

            if(usersInfo.Length == 0)
            {
                return new User[] { }; // TODO ?
            }

            return usersInfo.ToArray();
        }
    }
}
