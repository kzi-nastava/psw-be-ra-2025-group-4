using AutoMapper;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.Core.Domain;

namespace Explorer.Stakeholders.Core.Mappers
{
    public class DirectMessageProfile : Profile
    {
        public DirectMessageProfile()
        {
            CreateMap<DirectMessage, DirectMessageDto>()
                .ForMember(dest => dest.Sender, opt => opt.MapFrom(src => src.Sender.Username))
                .ForMember(dest => dest.Recipient, opt => opt.MapFrom(src => src.Recipient.Username));
        }
    }
}
