using AutoMapper;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.Core.Domain;

namespace Explorer.Stakeholders.Core.Mappers
{
    public class DirectMessageProfile : Profile
    {
        public DirectMessageProfile()
        {
            CreateMap<DirectMessageDto, DirectMessage>().ReverseMap();
        }
    }
}
