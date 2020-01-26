using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading.Tasks;
using VkNet.Abstractions;
using VkNet.Model.RequestParams;

namespace Goblin.Application.Extensions
{
    public static class MessageCategoryExtensions
    {
        private static readonly RandomNumberGenerator Rng = RandomNumberGenerator.Create();

        public static async Task SendError(this IMessagesCategory msgCategory, string error, long peerId)
        {
            await msgCategory.SendWithRandomId(new MessagesSendParams
            {
                PeerId = peerId,
                Message = $"❌ Ошибка: {error}"
            });
        }

        public static async Task SendWithRandomId(this IMessagesCategory msgCategory, MessagesSendParams @params)
        {
            AddKeyboard(@params);

            @params.RandomId = GetRandomId();
            await msgCategory.SendAsync(@params);
        }

        public static async Task SendErrorToUserIds(
                this IMessagesCategory msgCategory,
                string error, IEnumerable<long> userIds)
        {
            await msgCategory.SendToUserIdsWithRandomId(new MessagesSendParams
            {
                UserIds = userIds,
                Message = $"❌ Ошибка: {error}"
            });
        }

        public static async Task SendToUserIdsWithRandomId(
                this IMessagesCategory msgCategory, MessagesSendParams @params)
        {
            AddKeyboard(@params);

            @params.RandomId = GetRandomId();
            await msgCategory.SendToUserIdsAsync(@params);
        }

        private static void AddKeyboard(MessagesSendParams @params)
        {
            const int conversationsStartId = 2000000000;
            if(@params.Keyboard is null && @params.PeerId < conversationsStartId)
            {
                @params.Keyboard = DefaultKeyboards.GetDefaultKeyboard();
            }
        }

        private static int GetRandomId()
        {
            var intBytes = new byte[4];
            Rng.GetBytes(intBytes);
            return BitConverter.ToInt32(intBytes, 0);
        }
    }
}