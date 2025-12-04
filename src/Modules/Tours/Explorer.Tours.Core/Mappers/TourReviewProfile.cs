using AutoMapper;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.Core.Mappers
{
    public class TourReviewProfile : Profile
    {
        public TourReviewProfile()
        {
            CreateMap<TourReview, TourReviewDto>().ReverseMap();
            CreateMap<TourReview, TourReviewResponseDto>().ReverseMap();

            CreateMap<TourReviewCreateDto, TourReview>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.TouristId, opt => opt.Ignore())
                .ForMember(dest => dest.TourId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.CompletionPercentage, opt => opt.Ignore())
                .ForMember(dest => dest.LastModifiedAt, opt => opt.Ignore());
        }
    }
}