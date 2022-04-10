using AutoMapper;
using Goblin.Application.Core.Models;

namespace Goblin.WebApp;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<VkNet.Model.Message, Message>()
                .ForMember(x => x.Text,
                           x => x.MapFrom(m => m.Text))
                .ForMember(x => x.Payload,
                           x => x.MapFrom(m => m.Payload))
                .ForMember(x => x.ChatId,
                           x => x.MapFrom(m => m.PeerId))
                .ForMember(x => x.UserId, 
                           x => x.MapFrom(m => m.FromId))
                .ForMember(dst => dst.UserTag, src => src.MapFrom(x => $"@id{x.FromId}"));
        
        CreateMap<VkNet.Model.GroupUpdate.MessageEvent, Message>()
                .ForMember(x => x.Payload,
                           x => x.MapFrom(m => m.Payload))
                .ForMember(x => x.ChatId,
                           x => x.MapFrom(m => m.PeerId))
                .ForMember(x => x.UserId, 
                           x => x.MapFrom(m => m.UserId))
                .ForMember(dst => dst.UserTag, src => src.MapFrom(x => $"@id{x.UserId}"));

        CreateMap<Telegram.Bot.Types.Message, Message>()
                .ForMember(x => x.Text,
                           x => x.MapFrom(m => m.Text))
                // .ForMember(x => x.MessagePayload)
                .ForMember(x => x.ChatId,
                           x => x.MapFrom(m => m.Chat.Id))
                .ForMember(x => x.UserId,
                           x => x.MapFrom(m => m.From.Id))
                .ForMember(dst => dst.UserTag, src => src.MapFrom(x => $"@{x.Chat.Username} (`{x.Chat.Id}`)"));

        CreateMap<Telegram.Bot.Types.CallbackQuery, Message>()
                .ForMember(x => x.Text,
                           x => x.MapFrom(m => m.Message.Text))
                .ForMember(x => x.Payload,
                           x => x.MapFrom(m => m.Data))
                .ForMember(x => x.ChatId,
                           x => x.MapFrom(m => m.From.Id))
                .ForMember(x => x.UserId,
                           x => x.MapFrom(m => m.From.Id));
    }
}