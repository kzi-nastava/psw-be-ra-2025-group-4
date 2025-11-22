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
                .ForMember(
                    dest => dest.Sender,
                    opt => opt.MapFrom(src => src.Sender != null ? $"{src.Sender.Name} {src.Sender.Surname}" : null)
                )
                .ForMember(
                    dest => dest.Recipient,
                    opt => opt.MapFrom(src => src.Recipient != null ? $"{src.Recipient.Name} {src.Recipient.Surname}" : null)
                );
        }
    }
}
