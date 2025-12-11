using AutoMapper;
using Explorer.Blog.API.Dtos;
using Explorer.Blog.Core.Domain;
using System.Text.Json.Serialization;

namespace Explorer.Blog.Core.Mappers;

public class BlogProfile : Profile
{
    public BlogProfile()
    {
        CreateMap<DigitalDiaryDto, DigitalDiary>().ReverseMap();

        CreateMap<BlogPost, BlogDto>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => (BlogStatusDTO)src.Status))
            .ForMember(dest => dest.Popularity, opt => opt.MapFrom(src => (BlogPopularityDTO)src.Popularity))
            .ForMember(dest => dest.Images, opt => opt.MapFrom(src => src.Images))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
            .ForMember(dest => dest.LastUpdatedAt, opt => opt.MapFrom(src => src.LastUpdatedAt));

        CreateMap<CreateUpdateBlogDto, BlogPost>()
            .ForCtorParam("images", opt => opt.MapFrom(src => src.Images));
    }
}
