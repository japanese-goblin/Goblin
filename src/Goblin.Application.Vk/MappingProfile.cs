using AutoMapper;
using Goblin.Application.Vk.Models;
using VkNet.Model;

namespace Goblin.Application.Vk
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<VkMessage, Message>();
            CreateMap<Message, VkMessage>();
        }
    }
}