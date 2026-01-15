using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Encounters.API.Dtos;
using Explorer.Encounters.API.Public.Tourist;
using Explorer.Encounters.Core.Domain;
using Explorer.Encounters.Core.Domain.Repositories;
using Explorer.Encounters.Core.Domain.RepositoryInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using DomainEncounterType = Explorer.Encounters.Core.Domain.EncounterType;

namespace Explorer.Encounters.Core.UseCases
{
    public class TouristEncounterService : ITouristEncounterService
    {
        private readonly IEncounterRepository _encounterRepository;
        private readonly IHiddenLocationEncounterRepository _hiddenLocationEncounterRepository;
        private readonly IEncounterExecutionRepository _encounterExecutionRepository;

        public TouristEncounterService(
            IEncounterRepository encounterRepository,
            IHiddenLocationEncounterRepository hiddenLocationEncounterRepository,
            IEncounterExecutionRepository encounterExecutionRepository)
        {
            _encounterRepository = encounterRepository;
            _hiddenLocationEncounterRepository = hiddenLocationEncounterRepository;
            _encounterExecutionRepository = encounterExecutionRepository;
        }

        public List<EncounterViewDto> GetByTourPoint(long touristId, int tourPointId, LocationDto touristLocation)
        {
            var encounters = _encounterRepository.GetByTourPointIds(new List<int> { tourPointId });
            var touristLoc = ToDomainLocation(touristLocation);

            return encounters.Select(e =>
            {
                var execution = _encounterExecutionRepository.Get(touristId, e.Id);
                var isStarted = execution != null;
                var isCompleted = execution?.Status == EncounterExecutionStatus.Completed;

                bool canActivate = !isStarted && !isCompleted;

                // HiddenLocation: activation radius must be satisfied
                if (e.Type == DomainEncounterType.Location)
                {
                    var hidden = _hiddenLocationEncounterRepository.Get(e.Id);
                    if (hidden != null)
                    {
                        var dist = hidden.PhotoPoint.DistanceToMeters(touristLoc);
                        canActivate = canActivate && dist <= hidden.ActivationRadiusMeters;
                    }
                }

                var dto = new EncounterViewDto
                {
                    Id = e.Id,
                    Name = e.Name,
                    Description = e.Description,
                    Location = new LocationDto { Longitude = e.Location.Longitude, Latitude = e.Location.Latitude },
                    ExperiencePoints = e.ExperiencePoints,
                    Type = (Explorer.Encounters.API.Dtos.EncounterType)(int)e.Type,
                    TourPointId = e.TourPointId,
                    IsRequiredForPointCompletion = e.IsRequiredForPointCompletion,

                    CanActivate = canActivate,
                    IsStarted = isStarted,
                    IsCompleted = isCompleted
                };

                if (e.Type == DomainEncounterType.Location)
                {
                    var hidden = _hiddenLocationEncounterRepository.Get(e.Id);
                    if (hidden != null)
                    {
                        dto.ImageUrl = hidden.ImageUrl;
                        dto.PhotoPoint = new LocationDto { Longitude = hidden.PhotoPoint.Longitude, Latitude = hidden.PhotoPoint.Latitude };
                        dto.ActivationRadiusMeters = hidden.ActivationRadiusMeters;
                    }
                }

                return dto;
            }).ToList();
        }

        public void StartEncounter(long touristId, long encounterId)
        {
            // validate encounter exists
            _ = _encounterRepository.GetById(encounterId);

            var existing = _encounterExecutionRepository.Get(touristId, encounterId);
            if (existing != null) return;

            var execution = new EncounterExecution(touristId, encounterId);
            _encounterExecutionRepository.Create(execution);
        }

        public void UpdateLocation(long touristId, long encounterId, LocationDto touristLocation)
        {
            var encounter = _encounterRepository.GetById(encounterId);

            // IMPORTANT: do NOT auto-start; ignore if not activated
            var execution = _encounterExecutionRepository.Get(touristId, encounterId);
            if (execution == null) return;

            if (execution.Status == EncounterExecutionStatus.Completed) return;

            // Only HiddenLocation uses location strategy (5m/30s)
            if (encounter.Type != DomainEncounterType.Location) return;

            var hidden = _hiddenLocationEncounterRepository.Get(encounterId);
            if (hidden == null) return;

            var touristLoc = ToDomainLocation(touristLocation);

            var distanceToPhotoPoint = hidden.PhotoPoint.DistanceToMeters(touristLoc);
            var insideCompletionRadius = distanceToPhotoPoint <= hidden.CompletionRadiusMeters;

            if (!insideCompletionRadius)
            {
                if (execution.WithinRadiusSinceUtc != null)
                {
                    execution.LeaveRadius();
                    _encounterExecutionRepository.Update(execution);
                }
                return;
            }

            if (execution.WithinRadiusSinceUtc == null)
            {
                execution.EnterRadius();
                _encounterExecutionRepository.Update(execution);
                return;
            }

            var secondsInside = (DateTime.UtcNow - execution.WithinRadiusSinceUtc.Value).TotalSeconds;
            if (secondsInside >= hidden.CompletionHoldSeconds)
            {
                execution.Complete();
                _encounterExecutionRepository.Update(execution);
            }
        }

        public void CompleteEncounter(long touristId, long encounterId)
        {
            var encounter = _encounterRepository.GetById(encounterId);

            var execution = _encounterExecutionRepository.Get(touristId, encounterId);
            if (execution == null)
                throw new NotFoundException("Encounter execution not found. Activate encounter first.");

            if (execution.Status == EncounterExecutionStatus.Completed) return;

            // Misc (and any non-hidden): self-check completion (no proof)
            if (encounter.Type != DomainEncounterType.Location)
            {
                execution.Complete();
                _encounterExecutionRepository.Update(execution);
                return;
            }

            // Hidden: only if hold condition satisfied
            var hidden = _hiddenLocationEncounterRepository.Get(encounterId);
            if (hidden == null) return;

            if (execution.WithinRadiusSinceUtc == null) return;

            var secondsInside = (DateTime.UtcNow - execution.WithinRadiusSinceUtc.Value).TotalSeconds;
            if (secondsInside < hidden.CompletionHoldSeconds) return;

            execution.Complete();
            _encounterExecutionRepository.Update(execution);
        }

        private static Location ToDomainLocation(LocationDto dto)
        {
            return new Location(dto.Longitude, dto.Latitude);
        }
    }
}
