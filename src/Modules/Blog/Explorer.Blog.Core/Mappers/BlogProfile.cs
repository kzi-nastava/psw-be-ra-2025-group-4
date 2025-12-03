using AutoMapper;
using Explorer.Blog.API.Dtos;
using Explorer.Blog.Core.Domain;

namespace Explorer.Blog.Core.Mappers;

public class BlogProfile : Profile
{
    public BlogProfile()
    {
        CreateMap<DigitalDiaryDto, DigitalDiary>()
                .ReverseMap();
        CreateMap<BlogPost, BlogDto>()
                        .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
                        .ForMember(dest => dest.LastUpdatedAt, opt => opt.MapFrom(src => src.LastUpdatedAt));

        CreateMap<CreateUpdateBlogDto, BlogPost>()
            .ForCtorParam("images", opt => opt.MapFrom(src => src.Images));
    }
}