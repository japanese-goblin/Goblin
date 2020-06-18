using System;
using Goblin.Application.Core.Abstractions;
using VkNet.Model;

namespace Goblin.Application.Vk.Models
{
    public class VkMessage : Message, IMessage
    {
        public long MessageUserId => FromId ?? throw new ArgumentException();
        public long MessageChatId => PeerId ?? throw new ArgumentException();
    }
}