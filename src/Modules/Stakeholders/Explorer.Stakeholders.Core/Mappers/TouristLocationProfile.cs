using AutoMapper;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.Core.Domain;

namespace Explorer.Stakeholders.Core.Mappers
{
    public class TouristLocationProfile : Profile
    {
        public TouristLocationProfile()
        {
            CreateMap<TouristLocation, TouristLocationDto>().ReverseMap();
        }
    }
}
