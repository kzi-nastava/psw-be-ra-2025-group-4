using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Encounters.API.Dtos;
using Explorer.Encounters.API.Public.Tourist;
using Explorer.Encounters.Core.Domain;
using Explorer.Encounters.Core.Domain.Repositories;
using Explorer.Encounters.Core.Domain.RepositoryInterfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Security.Cryptography;
using DomainEncounterType = Explorer.Encounters.Core.Domain.EncounterType;

namespace Explorer.Encounters.Core.UseCases
{
    public class TouristEncounterService : ITouristEncounterService
    {
        private readonly IEncounterRepository _encounterRepository;
        private readonly IHiddenLocationEncounterRepository _hiddenLocationEncounterRepository;
        private readonly IEncounterExecutionRepository _encounterExecutionRepository;
        private readonly ISocialEncounterParticipantRepository _participantRepository;
        private readonly IEncounterParticipantRepository _encounterParticipantRepository;

        public TouristEncounterService(
            IEncounterRepository encounterRepository,
            IHiddenLocationEncounterRepository hiddenLocationEncounterRepository,
            IEncounterExecutionRepository encounterExecutionRepository,
            ISocialEncounterParticipantRepository socialEncounterParticipantRepository,
            IEncounterParticipantRepository encounterParticipantRepository)
        {
            _encounterRepository = encounterRepository;
            _hiddenLocationEncounterRepository = hiddenLocationEncounterRepository;
            _encounterExecutionRepository = encounterExecutionRepository;
            _participantRepository = socialEncounterParticipantRepository;
            _encounterParticipantRepository = encounterParticipantRepository;
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

        public int UpdateTouristLocation(long encounterId, long touristId, double lat, double lng)
        {
            var now = DateTime.UtcNow;

            var execution = _encounterExecutionRepository.Get(touristId, encounterId);
            if (execution == null) 
                throw new InvalidOperationException("Encounter not started.");

            if (execution.Status == EncounterExecutionStatus.Completed) 
                throw new InvalidOperationException("Encounter already completed.");

            var encounter = _encounterRepository.GetById(encounterId) as SocialEncounter;
            if (encounter == null)
                throw new InvalidOperationException("Not a SocialEncounter");

            var distance = CalculateDistanceMeters(lat, lng, encounter.Location.Latitude, encounter.Location.Longitude);
            var participant = _participantRepository.Get(encounterId, touristId);
        

            if (distance <= encounter.ActivationRadiusMeters)
            {
                if (participant == null)
                {
                    participant = new SocialEncounterParticipant(encounterId, touristId, now);
                    _participantRepository.Add(participant);
                }
                else
                {
      
                    participant.SetLastSeenAt(now);
                    _participantRepository.Update(participant);
                }
            }
            else
            {
                if (participant != null)
                {
                    _participantRepository.Remove(participant);
                }
            }

            //_participantRepository.RemoveStale(encounterId, TimeSpan.FromSeconds(20), now);
            var activeParticipants = _participantRepository.GetActive(encounterId, now).ToList();
            var activeCount = activeParticipants.Count;

            if (activeCount >= encounter.MinimumParticipants)
            {
               
                var touristIdsInRange = activeParticipants.Select(p => p.TouristId);
                _encounterExecutionRepository.ResolveEncounterForParticipants(encounterId, touristIdsInRange);
                foreach(int tId in touristIdsInRange)
                {
                    UpdateParticipance(tId, encounter.ExperiencePoints);
                }
            }

            return activeCount;
        }

        private void UpdateParticipance(int touristId, int xp)
        {
            var encounterParticipant = _encounterParticipantRepository.Get(touristId);
            if (encounterParticipant == null)
            {
                _encounterParticipantRepository.Add(new EncounterParticipant(touristId));
            }
            else
            {
                encounterParticipant.AddExperience(xp);
                _encounterParticipantRepository.Update(encounterParticipant);
            }
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
                UpdateParticipance((int)touristId, encounter.ExperiencePoints);
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
            UpdateParticipance((int)touristId, encounter.ExperiencePoints);
            _encounterExecutionRepository.Update(execution);
        }

        private static Location ToDomainLocation(LocationDto dto)
        {
            return new Location(dto.Longitude, dto.Latitude);
        }

        private double CalculateDistanceMeters(double lat1, double lon1, double lat2, double lon2)
        {
            var R = 6371000; 
            var dLat = DegreesToRadians(lat2 - lat1);
            var dLon = DegreesToRadians(lon2 - lon1);
            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(DegreesToRadians(lat1)) * Math.Cos(DegreesToRadians(lat2)) *
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return R * c;
        }

        private double DegreesToRadians(double deg) => deg * Math.PI / 180;

    }
}
