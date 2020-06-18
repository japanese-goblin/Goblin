using System;
using Goblin.Application.Core.Abstractions;
using VkNet.Model;

namespace Goblin.Application.Vk.Models
{
    public class VkMessage : Message, IMessage
    {
        public long FromUserId => FromId ?? throw new ArgumentException();
        public long ToUserId => PeerId ?? throw new ArgumentException();
    }
}