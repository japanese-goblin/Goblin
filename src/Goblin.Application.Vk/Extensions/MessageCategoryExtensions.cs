using System;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Goblin.Application.Core;
using Goblin.Application.Vk.Converters;
using VkNet.Abstractions;
using VkNet.Model.RequestParams;

namespace Goblin.Application.Vk.Extensions;

public static class MessageCategoryExtensions
{
    private static readonly RandomNumberGenerator Rng = RandomNumberGenerator.Create();

    public static async Task SendWithRandomId(this IMessagesCategory msgCategory, MessagesSendParams @params)
    {
        AddKeyboard(@params);

        @params.RandomId = GetRandomId();
        await msgCategory.SendAsync(@params);
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
            @params.Keyboard = KeyboardConverter.FromCoreToVk(DefaultKeyboards.GetDefaultKeyboard());
        }
    }

    private static int GetRandomId()
    {
        var intBytes = new byte[4];
        Rng.GetBytes(intBytes);
        return BitConverter.ToInt32(intBytes, 0);
    }
}