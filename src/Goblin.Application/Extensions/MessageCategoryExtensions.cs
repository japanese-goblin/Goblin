using System;
using System.Security.Cryptography;
using System.Threading.Tasks;
using VkNet.Abstractions;
using VkNet.Model.RequestParams;

namespace Goblin.Application.Extensions
{
    public static class MessageCategoryExtensions
    {
        private static readonly RandomNumberGenerator Rng = RandomNumberGenerator.Create();
        
        public static async Task<long> SendError(this IMessagesCategory msgCategory, string error, long peerId)
        {
            return await msgCategory.SendWithRandomId(new MessagesSendParams
            {
                PeerId = peerId,
                Message = $"❌ Ошибка: {error}"
            });
        }

        public static async Task<long> SendWithRandomId(this IMessagesCategory msgCategory, MessagesSendParams @params)
        {
            @params.RandomId = GetRandomId();
            return await msgCategory.SendAsync(@params);
        }
        
        private static int GetRandomId()
        {
            var intBytes = new byte[4];
            Rng.GetBytes(intBytes);
            return BitConverter.ToInt32(intBytes, 0);
        }
    }
}