using AutoMapper;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.Core.Domain;

namespace Explorer.Stakeholders.Core.Mappers
{
    public class NotificationProfile : Profile
    {
        public NotificationProfile()
        {
            CreateMap<Notification, NotificationDto>()
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type.ToString()))
                .ForMember(dest => dest.ResourceUrl, opt => opt.MapFrom(src => src.ResourceUrl))
                .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.Content))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
                .ForMember(dest => dest.IsRead, opt => opt.MapFrom(src => src.IsRead))
                .ForMember(dest => dest.ActorId, opt => opt.MapFrom(src => src.ActorId))
                .ForMember(dest => dest.ActorUsername, opt => opt.MapFrom(src => src.ActorUsername))
                .ForMember(d => d.Count, o => o.MapFrom(s => s.Count));
        }
    }
}
