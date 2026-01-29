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
        // Base DTO mappings
        CreateMap<Encounter, EncounterDto>()
            .Include<HiddenLocationEncounter, EncounterDto>()
            .Include<SocialEncounter, EncounterDto>()
            .Include<QuizEncounter, QuizEncounterDto>();

        CreateMap<EncounterDto, Encounter>();

        // Specific DTO mappings
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

        // Quiz-related DTO mappings
        CreateMap<QuizAnswer, EncounterQuizAnswerDto>().ReverseMap();
        CreateMap<QuizQuestion, QuizQuestionDto>().ReverseMap();
        CreateMap<QuizEncounter, QuizEncounterDto>()
            .IncludeBase<Encounter, EncounterDto>()
            .ReverseMap();

        // Other entity mappings
        CreateMap<Location, LocationDto>().ReverseMap();
        CreateMap<HiddenLocationEncounter, HiddenLocationEncounterDto>().ReverseMap();
        CreateMap<SocialEncounter, SocialEncounterDto>()
            .IncludeBase<Encounter, EncounterDto>();
        CreateMap<EncounterExecution, EncounterExecutionDto>().ReverseMap();
        CreateMap<EncounterParticipant, EncounterParticipantDto>().ReverseMap();

        // ===== VIEW DTO MAPPINGS (for tourist execution) =====

        // Base ViewDto mapping - FIXED: Added QuizEncounter to Include
        CreateMap<Encounter, EncounterViewDto>()
            .Include<HiddenLocationEncounter, EncounterViewDto>()
            .Include<SocialEncounter, EncounterViewDto>()
            .Include<QuizEncounter, EncounterViewDto>()  // ADDED THIS
            .ForMember(d => d.Id, m => m.MapFrom(s => s.Id))
            .ForMember(d => d.Name, m => m.MapFrom(s => s.Name))
            .ForMember(d => d.Description, m => m.MapFrom(s => s.Description))
            .ForMember(d => d.Location, m => m.MapFrom(s => s.Location))
            .ForMember(d => d.ExperiencePoints, m => m.MapFrom(s => s.ExperiencePoints))
            .ForMember(d => d.Type, m => m.MapFrom(s => s.Type))
            .ForMember(d => d.TourPointId, m => m.MapFrom(s => s.TourPointId))
            .ForMember(d => d.IsRequiredForPointCompletion, m => m.MapFrom(s => s.IsRequiredForPointCompletion))
            .ForMember(d => d.IsStarted, m => m.MapFrom(s => s.Status == Domain.EncounterStatus.Active))
            .ForMember(d => d.IsCompleted, m => m.MapFrom(s => s.Status == Domain.EncounterStatus.Archived))
            .ForMember(d => d.CanActivate, m => m.MapFrom(s =>
                s.Status == Domain.EncounterStatus.Draft &&
                s.ApprovalStatus == EncounterApprovalStatus.APPROVED))
            // Ignore subtype-specific fields in base mapping
            .ForMember(d => d.ImageUrl, m => m.Ignore())
            .ForMember(d => d.PhotoPoint, m => m.Ignore())
            .ForMember(d => d.ActivationRadiusMeters, m => m.Ignore())
            .ForMember(d => d.MinimumParticipants, m => m.Ignore())
            .ForMember(d => d.Questions, m => m.Ignore())
            .ForMember(d => d.TimeLimit, m => m.Ignore());

        // HiddenLocationEncounter ViewDto mapping
        CreateMap<HiddenLocationEncounter, EncounterViewDto>()
            .IncludeBase<Encounter, EncounterViewDto>()
            .ForMember(d => d.ImageUrl, m => m.MapFrom(s => s.ImageUrl))
            .ForMember(d => d.PhotoPoint, m => m.MapFrom(s => s.PhotoPoint))
            .ForMember(d => d.ActivationRadiusMeters, m => m.MapFrom(s => s.ActivationRadiusMeters))
            .ForMember(d => d.MinimumParticipants, m => m.MapFrom(_ => (int?)null))
            .ForMember(d => d.Questions, m => m.Ignore())
            .ForMember(d => d.TimeLimit, m => m.Ignore());

        // SocialEncounter ViewDto mapping
        CreateMap<SocialEncounter, EncounterViewDto>()
            .IncludeBase<Encounter, EncounterViewDto>()
            .ForMember(d => d.MinimumParticipants, m => m.MapFrom(s => s.MinimumParticipants))
            .ForMember(d => d.ActivationRadiusMeters, m => m.MapFrom(s => s.ActivationRadiusMeters))
            .ForMember(d => d.ImageUrl, m => m.MapFrom(_ => (string?)null))
            .ForMember(d => d.PhotoPoint, m => m.MapFrom(_ => (LocationDto?)null))
            .ForMember(d => d.Questions, m => m.Ignore())
            .ForMember(d => d.TimeLimit, m => m.Ignore());

        // QuizEncounter ViewDto mapping - FIXED: Added IncludeBase and ignore irrelevant fields
        CreateMap<QuizEncounter, EncounterViewDto>()
            .IncludeBase<Encounter, EncounterViewDto>()  // ADDED THIS
            .ForMember(d => d.Questions, m => m.MapFrom(s => s.Questions))
            .ForMember(d => d.TimeLimit, m => m.MapFrom(s => s.TimeLimit))
            // Ignore fields that don't apply to Quiz
            .ForMember(d => d.ImageUrl, m => m.Ignore())
            .ForMember(d => d.PhotoPoint, m => m.Ignore())
            .ForMember(d => d.ActivationRadiusMeters, m => m.Ignore())
            .ForMember(d => d.MinimumParticipants, m => m.Ignore());

        // Quiz question/answer ViewDto mappings - ADDED THESE
        CreateMap<QuizQuestion, QuizQuestionDto>()
            .ForMember(d => d.Id, m => m.MapFrom(s => s.Id))
            .ForMember(d => d.Text, m => m.MapFrom(s => s.Text))
            .ForMember(d => d.Answers, m => m.MapFrom(s => s.Answers));

        CreateMap<QuizAnswer, EncounterQuizAnswerDto>()
            .ForMember(d => d.Id, m => m.MapFrom(s => s.Id))
            .ForMember(d => d.Text, m => m.MapFrom(s => s.Text))
            .ForMember(d => d.IsCorrect, m => m.MapFrom(s => s.IsCorrect));
    }
}