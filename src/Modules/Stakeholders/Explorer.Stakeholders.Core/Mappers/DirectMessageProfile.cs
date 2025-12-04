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
                    opt => opt.MapFrom(src => src.Sender != null ? src.Sender.Username : null)
                )
                .ForMember(
                    dest => dest.Recipient,
                    opt => opt.MapFrom(src => src.Recipient != null ? src.Recipient.Username : null)
                )
                .ForMember(
                    dest => dest.ResourceUrl,
                    opt => opt.MapFrom(src =>
                        src.ResourceType == ResourceType.Blog && src.ResourceId != null
                            ? $"/blog/{src.ResourceId}"
                            : src.ResourceType == ResourceType.Tour && src.ResourceId != null
                                ? $"/tours/{src.ResourceId}"
                                : null
                    )
                );
        }
    }
}
