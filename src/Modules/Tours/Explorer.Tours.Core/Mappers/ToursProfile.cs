using AutoMapper;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.Core.Domain;

namespace Explorer.Tours.Core.Mappers
{
    public class ToursProfile : Profile
    {
        public ToursProfile()
        {
            CreateMap<EquipmentDto, Equipment>().ReverseMap();
            CreateMap<TourStatus, TourDtoStatus>().ConvertUsing(src => (TourDtoStatus)src);
            CreateMap<TourDtoStatus, TourStatus>().ConvertUsing(src => (TourStatus)src);
            CreateMap<Tour, TourDto>().ReverseMap();
            CreateMap<CreateUpdateTourDto, Tour>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.AuthorId, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.Ignore())
                .ForMember(dest => dest.Price, opt => opt.Ignore());

            CreateMap<HistoricalMonumentDTO, HistoricalMonument>().ForMember(dest => dest.Status, opt => opt.MapFrom(src => (MonumentStatus)src.Status));

            CreateMap<HistoricalMonument, HistoricalMonumentDTO>().ForMember(dest => dest.Status, opt => opt.MapFrom(src => (MonumentStatusDTO)src.Status));


            CreateMap<TouristEquipmentDTO, TouristEquipment>().ReverseMap();

            CreateMap<TourProblemDto, TourProblem>().ReverseMap();
            CreateMap<ProblemCategoryDto, ProblemCategory>().ConvertUsing(src => (ProblemCategory)src);
            CreateMap<ProblemCategory, ProblemCategoryDto>().ConvertUsing(src => (ProblemCategoryDto)src);
            CreateMap<ProblemPriorityDto, ProblemPriority>().ConvertUsing(src => (ProblemPriority)src);
            CreateMap<ProblemPriority, ProblemPriorityDto>().ConvertUsing(src => (ProblemPriorityDto)src);
        }
    }
}