using AutoMapper;
using Goblin.Application.Telegram.Models;
using Goblin.Application.Vk.Models;
using VkNet.Model;

namespace Goblin.WebApp
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<VkMessage, Message>();
            CreateMap<Message, VkMessage>();

            CreateMap<TelegramMessage, Telegram.Bot.Types.Message>();
            CreateMap<Telegram.Bot.Types.Message, TelegramMessage>();
        }
    }
}