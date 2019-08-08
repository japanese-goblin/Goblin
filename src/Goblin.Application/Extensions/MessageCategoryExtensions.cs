using System;
using System.Security.Cryptography;
using VkNet.Abstractions;
using VkNet.Model.RequestParams;

namespace Goblin.Application.Extensions
{
    public static class MessageCategoryExtensions
    {
        private static readonly RandomNumberGenerator Rng = RandomNumberGenerator.Create();
        
        public static long SendError(this IMessagesCategory msgCategory, string error, long peerId)
        {
            return msgCategory.SendWithRandomId(new MessagesSendParams
            {
                PeerId = peerId,
                Message = $"❌ Ошибка: {error}"
            });
        }

        public static long SendWithRandomId(this IMessagesCategory msgCategory, MessagesSendParams @params)
        {
            @params.RandomId = GetRandomId();
            return msgCategory.Send(@params);
        }
        
        private static int GetRandomId()
        {
            var intBytes = new byte[4];
            Rng.GetBytes(intBytes);
            return BitConverter.ToInt32(intBytes, 0);
        }
    }
}