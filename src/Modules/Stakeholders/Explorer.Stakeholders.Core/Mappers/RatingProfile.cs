using AutoMapper;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.Core.Domain;

namespace Explorer.Stakeholders.Core.Mappers
{
    public class RatingProfile : Profile
    {
        public RatingProfile()
        {
            CreateMap<Rating, RatingDto>().ReverseMap();

            CreateMap<RatingCreateDto, Rating>();
        }
    }
}
