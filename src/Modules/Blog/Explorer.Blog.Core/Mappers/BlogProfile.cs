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
        CreateMap<BlogPost, BlogDto>();

        CreateMap<CreateUpdateBlogDto, BlogPost>()
            .ForCtorParam("images", opt => opt.MapFrom(src => src.Images));
    }
}