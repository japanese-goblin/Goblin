using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Vk.Models.Keyboard;

namespace Vk.Category
{
    public class Messages
    {
        private readonly VkApi _api;

        public Messages(VkApi api)
        {
            _api = api;
        }

        public async Task Send(long id, string text, string[] attachs = null, Keyboard kb = null)
        {
            await Send(new[] { id }, text, attachs, kb);
        }

        public async Task Send(long[] ids, string text, string[] attachs = null, Keyboard kb = null)
        {
            if(ids.Length == 0)
            {
                throw new ArgumentNullException(nameof(ids), "Укажите хотя бы один peer_id");
            }
            if(string.IsNullOrEmpty(text) && attachs is null)
            {
                throw new ArgumentNullException(nameof(text), "Укажите текст или прикрепления");
            }

            var values = new Dictionary<string, string>
            {
                ["message"] = text,
                ["random_id"] = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString(), //TODO
                ["peer_ids"] = string.Join(',', ids)
            };

            if(!(attachs is null)) // если есть аттачи
            {
                values.Add("attachment", string.Join(",", attachs));
            }

            if(!(kb is null)) // если есть клавиатура
            {
                values.Add("keyboard", kb.ToString());
            }

            await _api.CallApi("messages.send", values); //TODO to bool?
        }
    }
}
