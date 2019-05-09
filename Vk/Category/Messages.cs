using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Vk.Models.Keyboard;
using Vk.Models.Messages;

namespace Vk.Category
{
    public class Messages
    {
        private readonly VkApi _api;

        public Messages(VkApi api)
        {
            _api = api;
        }

        public async Task<MessageSendResponse> Send(long id, string text, string[] attachs = null, Keyboard kb = null)
        {
            return (await Send(new[] { id }, text, attachs, kb))[0];
        }

        public async Task<MessageSendResponse[]> Send(IEnumerable<long> ids, string text, string[] attachs = null,
                                                      Keyboard kb = null)
        {
            if(!ids.Any())
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
                ["random_id"] = GetRandomId().ToString(),
                ["peer_ids"] = string.Join(',', ids)
            };

            if(attachs != null) // если есть аттачи
            {
                values.Add("attachment", string.Join(",", attachs));
            }

            if(kb != null) // если есть клавиатура
            {
                values.Add("keyboard", kb.ToString());
            }

            var result = await _api.CallApi("messages.send", values);
            return JsonConvert.DeserializeObject<MessageSendResponse[]>(result);
        }

        public async Task Delete(long messageIds, bool isSpam = false, bool deleteForAll = true)
        {
            await Delete(new[] { messageIds }, isSpam, deleteForAll);
        }

        public async Task Delete(long[] messageIds, bool isSpam = false, bool deleteForAll = true)
        {
            var values = new Dictionary<string, string>
            {
                ["message_ids"] = string.Join(',', messageIds),
                ["spam"] = (isSpam ? 1 : 0).ToString(), //TODO
                ["delete_for_all"] = (deleteForAll ? 1 : 0).ToString()
            };

            await _api.CallApi("messages.delete", values); //TODO to bool?
        }

        private static readonly RandomNumberGenerator Rng = RandomNumberGenerator.Create();


        private static int GetRandomId()
        {
            var intBytes = new byte[4];
            Rng.GetBytes(intBytes);
            return BitConverter.ToInt32(intBytes, 0);
        }
    }
}