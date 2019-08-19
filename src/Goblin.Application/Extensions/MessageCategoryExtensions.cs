using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Security.Cryptography;
using System.Threading.Tasks;
using VkNet.Abstractions;
using VkNet.Model;
using VkNet.Model.Keyboard;
using VkNet.Model.RequestParams;

namespace Goblin.Application.Extensions
{
    public static class MessageCategoryExtensions
    {
        private static readonly RandomNumberGenerator Rng = RandomNumberGenerator.Create();

        public static async Task<long> SendError(this IMessagesCategory msgCategory, string error, long peerId)
        {
            var kb = new KeyboardBuilder(true);
            kb.AddReturnToMenuButton(false);

            return await msgCategory.SendWithRandomId(new MessagesSendParams
            {
                PeerId = peerId,
                Message = $"❌ Ошибка: {error}",
                Keyboard = kb.Build()
            });
        }

        public static async Task<long> SendWithRandomId(this IMessagesCategory msgCategory, MessagesSendParams @params)
        {
            if(@params.Keyboard is null)
            {
                var kb = new KeyboardBuilder(true);
                kb.AddReturnToMenuButton(false);
                @params.Keyboard = kb.Build();
            }

            @params.RandomId = GetRandomId();
            return await msgCategory.SendAsync(@params);
        }

        public static async Task<ReadOnlyCollection<MessagesSendResult>> SendErrorToUserIds(
                this IMessagesCategory msgCategory,
                string error, IEnumerable<long> userIds)
        {
            var kb = new KeyboardBuilder(true);
            kb.AddReturnToMenuButton(false);

            return await msgCategory.SendToUserIdsWithRandomId(new MessagesSendParams
            {
                UserIds = userIds,
                Message = $"❌ Ошибка: {error}",
                Keyboard = kb.Build()
            });
        }

        public static async Task<ReadOnlyCollection<MessagesSendResult>> SendToUserIdsWithRandomId(
                this IMessagesCategory msgCategory, MessagesSendParams @params)
        {
            if(@params.Keyboard is null)
            {
                var kb = new KeyboardBuilder(true);
                kb.AddReturnToMenuButton(false);
                @params.Keyboard = kb.Build();
            }

            @params.RandomId = GetRandomId();
            return await msgCategory.SendToUserIdsAsync(@params);
        }

        private static int GetRandomId()
        {
            var intBytes = new byte[4];
            Rng.GetBytes(intBytes);
            return BitConverter.ToInt32(intBytes, 0);
        }
    }
}