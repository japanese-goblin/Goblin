using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
            return (await Get(new[] { id }))[0]; //TODO по идее, всегда должен быть 0 элемент
        }

        public async Task<User[]> Get(long[] ids)
        {
            if(ids.Length == 0)
            {
                throw new ArgumentException("ids пустой");
            }

            var values = new Dictionary<string, string>
            {
                ["user_ids"] = string.Join(',', ids),
                ["fields"] = "photo_200_orig"
            };

            var usersInfo = await _api.CallApi<User[]>("users.get", values);

            if(usersInfo.Length == 0)
            {
                return new User[] { }; // TODO ?
            }

            return usersInfo.ToArray();
        }
    }
}