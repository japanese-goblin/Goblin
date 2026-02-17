using Goblin.Domain;
using VkNet.Model;
using Message = Goblin.Application.Core.Models.Message;

namespace Goblin.Application.Vk.Converters;

public static class MessageConverter
{
    public static Message MapToBotMessage(this VkNet.Model.Message message)
    {
        return new Message
        {
            Payload = message.Payload,
            ConsumerType = ConsumerType.Vkontakte,
            Text = message.Text,
            ChatId = message.PeerId.GetValueOrDefault(),
            UserId = message.FromId.GetValueOrDefault(),
            UserTag = $"@id{message.FromId}"
        };
    }

    public static Message MapToBotMessage(this MessageEvent message)
    {
        return new Message
        {
            Payload = message.Payload,
            ConsumerType = ConsumerType.Vkontakte,
            ChatId = message.PeerId.GetValueOrDefault(),
            UserId = message.UserId.GetValueOrDefault(),
            UserTag = $"@id{message.UserId}"
        };
    }
}