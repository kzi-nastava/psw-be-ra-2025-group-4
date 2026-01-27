using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Explorer.Encounters.API.Dtos;
using Explorer.Encounters.Core.Domain;

namespace Explorer.Encounters.Core.Mappers;

public class EncountersProfile : Profile
{
    public EncountersProfile()
    {
        CreateMap<Encounter, EncounterDto>()
            .Include<HiddenLocationEncounter, EncounterDto>()
            .Include<SocialEncounter, EncounterDto>()
            .Include<QuizEncounter, QuizEncounterDto>();

        CreateMap<EncounterDto, Encounter>();



        CreateMap<HiddenLocationEncounter, EncounterDto>()
            .IncludeBase<Encounter, EncounterDto>()
            .ForMember(d => d.ImageUrl, o => o.MapFrom(s => s.ImageUrl))
            .ForMember(d => d.PhotoPoint, o => o.MapFrom(s => s.PhotoPoint))
            .ForMember(d => d.ActivationRadiusMeters, o => o.MapFrom(s => s.ActivationRadiusMeters))
            .ForMember(d => d.MinimumParticipants, o => o.Ignore());

        CreateMap<SocialEncounter, EncounterDto>()
            .IncludeBase<Encounter, EncounterDto>()
            .ForMember(d => d.MinimumParticipants, o => o.MapFrom(s => s.MinimumParticipants))
            .ForMember(d => d.ActivationRadiusMeters, o => o.MapFrom(s => s.ActivationRadiusMeters))
            .ForMember(d => d.ImageUrl, o => o.Ignore())
            .ForMember(d => d.PhotoPoint, o => o.Ignore());

        CreateMap<QuizAnswer, QuizAnswerDto>().ReverseMap();
        CreateMap<QuizQuestion, QuizQuestionDto>().ReverseMap();
        CreateMap<QuizEncounter, QuizEncounterDto>()
            .IncludeBase<Encounter, EncounterDto>()
            .ReverseMap();

        CreateMap<Location, LocationDto>().ReverseMap();
        CreateMap<HiddenLocationEncounter, HiddenLocationEncounterDto>().ReverseMap();
        CreateMap<SocialEncounter, SocialEncounterDto>()
            .IncludeBase<Encounter, EncounterDto>();
        CreateMap<EncounterExecution, EncounterExecutionDto>().ReverseMap();

        CreateMap<EncounterParticipant, EncounterParticipantDto>().ReverseMap();

        

        CreateMap<Encounter, EncounterViewDto>()
    .Include<HiddenLocationEncounter, EncounterViewDto>()
    .Include<SocialEncounter, EncounterViewDto>()

    // base fields
    .ForMember(d => d.Id, m => m.MapFrom(s => s.Id))
    .ForMember(d => d.Name, m => m.MapFrom(s => s.Name))
    .ForMember(d => d.Description, m => m.MapFrom(s => s.Description))
    .ForMember(d => d.Location, m => m.MapFrom(s => s.Location))
    .ForMember(d => d.ExperiencePoints, m => m.MapFrom(s => s.ExperiencePoints))
    .ForMember(d => d.Type, m => m.MapFrom(s => s.Type))
    .ForMember(d => d.TourPointId, m => m.MapFrom(s => s.TourPointId))
    .ForMember(d => d.IsRequiredForPointCompletion, m => m.MapFrom(s => s.IsRequiredForPointCompletion))

    // derived state flags
    .ForMember(d => d.IsStarted,
        m => m.MapFrom(s => s.Status == Domain.EncounterStatus.Active))

    .ForMember(d => d.IsCompleted,
        m => m.MapFrom(s => s.Status == Domain.EncounterStatus.Archived))

    // activation rule (intentionally strict)
    .ForMember(d => d.CanActivate,
        m => m.MapFrom(s =>
            s.Status == Domain.EncounterStatus.Draft &&
            s.ApprovalStatus == EncounterApprovalStatus.APPROVED))

    // defaults for subtype-only fields
        .ForMember(d => d.ImageUrl, m => m.Ignore())
        .ForMember(d => d.PhotoPoint, m => m.Ignore())
        .ForMember(d => d.ActivationRadiusMeters, m => m.Ignore())
        .ForMember(d => d.MinimumParticipants, m => m.Ignore());

        CreateMap<HiddenLocationEncounter, EncounterViewDto>()
            .ForMember(d => d.ImageUrl,
                m => m.MapFrom(s => s.ImageUrl))

            .ForMember(d => d.PhotoPoint,
                m => m.MapFrom(s => s.PhotoPoint))

            .ForMember(d => d.ActivationRadiusMeters,
                m => m.MapFrom(s => s.ActivationRadiusMeters))

            .ForMember(d => d.MinimumParticipants,
                m => m.MapFrom(_ => (int?)null));

        CreateMap<SocialEncounter, EncounterViewDto>()
            .ForMember(d => d.MinimumParticipants,
                m => m.MapFrom(s => s.MinimumParticipants))

            .ForMember(d => d.ActivationRadiusMeters,
                m => m.MapFrom(s => s.ActivationRadiusMeters))

            .ForMember(d => d.ImageUrl,
                m => m.MapFrom(_ => (string?)null))
            .ForMember(d => d.PhotoPoint,
                m => m.MapFrom(_ => (LocationDto?)null));

        CreateMap<QuizEncounter, EncounterViewDto>()
            .ForMember(d => d.Questions,
                           m => m.MapFrom(s => s.Questions.SelectMany(q => q.Answers).ToList()))
            .ForMember(d => d.TimeLimit, m => m.MapFrom(s => s.TimeLimit));
    }
}
