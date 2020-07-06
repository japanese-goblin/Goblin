using AutoMapper;
using Message = Goblin.Application.Core.Models.Message;

namespace Goblin.WebApp
{
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
                               x => x.MapFrom(m => m.FromId));

            CreateMap<Telegram.Bot.Types.Message, Message>()
                    .ForMember(x => x.Text,
                               x => x.MapFrom(m => m.Text))
                    // .ForMember(x => x.MessagePayload)
                    .ForMember(x => x.ChatId,
                               x => x.MapFrom(m => m.Chat.Id))
                    .ForMember(x => x.UserId,
                               x => x.MapFrom(m => m.From.Id));

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
}