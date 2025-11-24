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
            CreateMap<FacilityDto, Facility>().ReverseMap();

            CreateMap<TourStatus, TourDtoStatus>().ConvertUsing(src => (TourDtoStatus)src);
            CreateMap<TourDtoStatus, TourStatus>().ConvertUsing(src => (TourStatus)src);
            CreateMap<Tour, TourDto>().ReverseMap();


            CreateMap<TourPreferences, TourPreferencesDTO>()
            .ForMember(dest => dest.PreferredDifficulty,
               opt => opt.MapFrom(src => src.PreferredDifficulty.ToString()));


            CreateMap<CreateUpdateTourDto, Tour>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.AuthorId, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.Ignore())
                .ForMember(dest => dest.Price, opt => opt.Ignore());


            CreateMap<TouristEquipmentDTO, TouristEquipment>().ReverseMap();

            CreateMap<TourProblemDto, TourProblem>().ReverseMap();
            CreateMap<ProblemCategoryDto, ProblemCategory>().ConvertUsing(src => (ProblemCategory)src);
            CreateMap<ProblemCategory, ProblemCategoryDto>().ConvertUsing(src => (ProblemCategoryDto)src);
            CreateMap<ProblemPriorityDto, ProblemPriority>().ConvertUsing(src => (ProblemPriority)src);
            CreateMap<ProblemPriority, ProblemPriorityDto>().ConvertUsing(src => (ProblemPriorityDto)src);

        }
    }
}