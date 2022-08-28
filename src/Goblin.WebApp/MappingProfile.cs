using AutoMapper;
using Goblin.Domain;
using Telegram.Bot.Types;
using VkNet.Model.GroupUpdate;
using Message = VkNet.Model.Message;

namespace Goblin.WebApp;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Message, Application.Core.Models.Message>()
                .ForMember(x => x.Text,
                           x => x.MapFrom(m => m.Text))
                .ForMember(x => x.Payload,
                           x => x.MapFrom(m => m.Payload))
                .ForMember(x => x.ChatId,
                           x => x.MapFrom(m => m.PeerId))
                .ForMember(x => x.UserId,
                           x => x.MapFrom(m => m.FromId))
                .ForMember(dst => dst.UserTag, src => src.MapFrom(x => $"@id{x.FromId}"))
                .ForMember(dst => dst.ConsumerType, src => src.MapFrom(_ => ConsumerType.Vkontakte));

        CreateMap<MessageEvent, Application.Core.Models.Message>()
                .ForMember(x => x.Payload,
                           x => x.MapFrom(m => m.Payload))
                .ForMember(x => x.ChatId,
                           x => x.MapFrom(m => m.PeerId))
                .ForMember(x => x.UserId,
                           x => x.MapFrom(m => m.UserId))
                .ForMember(dst => dst.UserTag, src => src.MapFrom(x => $"@id{x.UserId}"))
                .ForMember(dst => dst.ConsumerType, src => src.MapFrom(_ => ConsumerType.Vkontakte));

        CreateMap<Telegram.Bot.Types.Message, Application.Core.Models.Message>()
                .ForMember(x => x.Text,
                           x => x.MapFrom(m => m.Text))
                // .ForMember(x => x.MessagePayload)
                .ForMember(x => x.ChatId,
                           x => x.MapFrom(m => m.Chat.Id))
                .ForMember(x => x.UserId,
                           x => x.MapFrom(m => m.From.Id))
                .ForMember(dst => dst.UserTag, src => src.MapFrom(x => $"@{x.Chat.Username} (`{x.Chat.Id}`)"))
                .ForMember(dst => dst.ConsumerType, src => src.MapFrom(_ => ConsumerType.Telegram));

        CreateMap<CallbackQuery, Application.Core.Models.Message>()
                .ForMember(x => x.Text,
                           x => x.MapFrom(m => m.Message.Text))
                .ForMember(x => x.Payload,
                           x => x.MapFrom(m => m.Data))
                .ForMember(x => x.ChatId,
                           x => x.MapFrom(m => m.From.Id))
                .ForMember(x => x.UserId,
                           x => x.MapFrom(m => m.From.Id))
                .ForMember(dst => dst.ConsumerType, src => src.MapFrom(_ => ConsumerType.Telegram));
    }
}