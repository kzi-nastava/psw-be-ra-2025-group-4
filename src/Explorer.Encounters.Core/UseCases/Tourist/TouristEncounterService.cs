using AutoMapper;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Encounters.API.Dtos;
using Explorer.Encounters.API.Public.Tourist;
using Explorer.Encounters.Core.Domain;
using Explorer.Encounters.Core.Domain.RepositoryInterfaces;
using DomainEncounterType = Explorer.Encounters.Core.Domain.EncounterType;

namespace Explorer.Encounters.Core.UseCases
{
    public class TouristEncounterService : ITouristEncounterService
    {
        private readonly IEncounterRepository _encounterRepository;
        private readonly IEncounterExecutionRepository _encounterExecutionRepository;
        private readonly IEncounterParticipantRepository _encounterParticipantRepository;
        private readonly IMapper _mapper;

        public TouristEncounterService(
            IEncounterRepository encounterRepository,
            IEncounterExecutionRepository encounterExecutionRepository,
            IEncounterParticipantRepository encounterParticipantRepository,
            IMapper mapper)
        {
            _encounterRepository = encounterRepository;
            _encounterExecutionRepository = encounterExecutionRepository;
            _encounterParticipantRepository = encounterParticipantRepository;
            _mapper = mapper;
        }

        public List<EncounterViewDto> GetByTourPoint(long touristId, int tourPointId, LocationDto touristLocation)
        {
            var touristLoc = ToDomainLocation(touristLocation);

            return _encounterRepository
                .GetByTourPointIds(new List<int> { tourPointId })
                .Select(encounter => CreateEncounterView(
                    encounter,
                    touristId,
                    touristLoc))
                .ToList();
        }

        private EncounterViewDto CreateEncounterView(
            Encounter encounter,
            long touristId,
            Location touristLocation)
        {
            var execution = _encounterExecutionRepository.Get(touristId, encounter.Id);

            bool isStarted = execution != null;
            bool isCompleted = execution?.Status == EncounterExecutionStatus.Completed;

            bool canActivate = CanActivateEncounter(
                encounter,
                isStarted,
                isCompleted,
                touristLocation);

            var dto = CreateBaseDto(encounter, isStarted, isCompleted, canActivate);

            EnrichDtoByEncounterType(encounter, dto);

            return dto;
        }

        private static EncounterViewDto CreateBaseDto(
            Encounter encounter,
            bool isStarted,
            bool isCompleted,
            bool canActivate)
        {
            return new EncounterViewDto
            {
                Id = encounter.Id,
                Name = encounter.Name,
                Description = encounter.Description,
                Location = new LocationDto
                {
                    Longitude = encounter.Location.Longitude,
                    Latitude = encounter.Location.Latitude
                },
                ExperiencePoints = encounter.ExperiencePoints,
                Type = (Explorer.Encounters.API.Dtos.EncounterType)(int)encounter.Type,
                TourPointId = encounter.TourPointId,
                IsRequiredForPointCompletion = encounter.IsRequiredForPointCompletion,

                CanActivate = canActivate,
                IsStarted = isStarted,
                IsCompleted = isCompleted
            };
        }

        private static void EnrichDtoByEncounterType(
            Encounter encounter,
            EncounterViewDto dto)
        {
            switch (encounter)
            {
                case HiddenLocationEncounter hidden:
                    dto.ImageUrl = hidden.ImageUrl;
                    dto.PhotoPoint = new LocationDto
                    {
                        Longitude = hidden.PhotoPoint.Longitude,
                        Latitude = hidden.PhotoPoint.Latitude
                    };
                    dto.ActivationRadiusMeters = hidden.ActivationRadiusMeters;
                    break;

                case SocialEncounter social:
                    dto.MinimumParticipants = social.MinimumParticipants;
                    dto.ActivationRadiusMeters = social.ActivationRadiusMeters;
                    break;

                case QuizEncounter quiz:
                    dto.TimeLimit = quiz.TimeLimit;
                    dto.Questions = quiz.Questions?
                        .Select(q => new QuizQuestionDto
                        {
                            Id = q.Id,
                            Text = q.Text,
                            Answers = q.Answers?
                                .Select(a => new EncounterQuizAnswerDto
                                {
                                    Id = a.Id,
                                    Text = a.Text,
                                    IsCorrect = a.IsCorrect
                                })
                                .ToList()
                        })
                        .ToList();
                    break;
            }
        }

        private bool CanActivateEncounter(
            Encounter encounter,
            bool isStarted,
            bool isCompleted,
            Location touristLocation)
        {
            if (isStarted || isCompleted)
                return false;

            switch (encounter)
            {
                case HiddenLocationEncounter hidden:
                    var hiddenDistance = hidden.PhotoPoint.DistanceToMeters(touristLocation);
                    return hiddenDistance <= hidden.ActivationRadiusMeters;

                case SocialEncounter social:
                    var socialDistance = social.Location.DistanceToMeters(touristLocation);
                    return socialDistance <= social.ActivationRadiusMeters;

                default:
                    return true;
            }
        }
        public void StartEncounter(long touristId, long encounterId)
        {
            var encounter = _encounterRepository.GetById(encounterId);
            if (encounter == null)
                throw new ArgumentException("Encounter not found.");

            var existing = _encounterExecutionRepository.Get(touristId, encounterId);
            if (existing != null)
                throw new InvalidOperationException("Encounter already started by this tourist.");

            var execution = new EncounterExecution(touristId, encounterId);
            _encounterExecutionRepository.Create(execution);
        }


        private void UpdateParticipance(int touristId, int xp)
        {
            var encounterParticipant = _encounterParticipantRepository.Get(touristId);
            if (encounterParticipant == null)
            {
                var participant = new EncounterParticipant(touristId);
                participant.AddExperience(xp);
                _encounterParticipantRepository.Add(participant);
            }
            else
            {
                encounterParticipant.AddExperience(xp);
                _encounterParticipantRepository.Update(encounterParticipant);
            }
        }

        public EncounterUpdateResultDto UpdateSocialEncounter(long touristId, long encounterId, LocationDto touristLocation)
        {
            var encounter = _encounterRepository.GetById(encounterId) as SocialEncounter;
            if (encounter == null)
                throw new ArgumentException("Encounter not found.");

            var execution = _encounterExecutionRepository.Get(touristId, encounterId);
            if (execution == null)
                throw new InvalidOperationException("Encounter execution does not exist!");

            if (execution.Status == EncounterExecutionStatus.Completed)
                throw new InvalidOperationException("Encounter already completed!");

            var touristLoc = ToDomainLocation(touristLocation);
            var distanceToEncounter = encounter.Location.DistanceToMeters(touristLoc);
            var insideCompletionRadius = distanceToEncounter <= encounter.ActivationRadiusMeters;

            if (!insideCompletionRadius)
            {
                if (execution.WithinRadiusSinceUtc != null)
                {
                    execution.LeaveRadius();
                    _encounterExecutionRepository.Update(execution);
                }
                return new EncounterUpdateResultDto { IsCompleted = false };
            }

            if (execution.WithinRadiusSinceUtc == null)
            {
                execution.EnterRadius();
                _encounterExecutionRepository.Update(execution); 
            }

            var activeExecutions = _encounterExecutionRepository.GetActiveExecutions(encounterId);

            if (activeExecutions.Count() >= encounter.MinimumParticipants)
            {
                foreach (EncounterExecution activeExecution in activeExecutions)
                {
                    activeExecution.Complete();
                    UpdateParticipance((int)activeExecution.TouristId, encounter.ExperiencePoints);
                    _encounterExecutionRepository.Update(activeExecution);
                }
                return new EncounterUpdateResultDto
                {
                    IsCompleted = true,
                    ExperiencePointsGained = encounter.ExperiencePoints
                };
            }
            return new EncounterUpdateResultDto { IsCompleted = false };
        }

        public EncounterUpdateResultDto UpdateLocationHiddenEncounter(long touristId, long encounterId, LocationDto touristLocation)
        {
            var encounter = _encounterRepository.GetById(encounterId) as HiddenLocationEncounter;
            if (encounter == null)
                throw new ArgumentException("Encounter not found.");

            var execution = _encounterExecutionRepository.Get(touristId, encounterId);
            if (execution == null)
                throw new InvalidOperationException("Encounter execution does not exist!");

            if (execution.Status == EncounterExecutionStatus.Completed)
                throw new InvalidOperationException("Encounter already completed!");
            var touristLoc = ToDomainLocation(touristLocation);
            var distanceToPhotoPoint = encounter.PhotoPoint.DistanceToMeters(touristLoc);
            var insideCompletionRadius = distanceToPhotoPoint <= encounter.CompletionRadiusMeters;


            if (!insideCompletionRadius)
            {
                if (execution.WithinRadiusSinceUtc != null)
                {
                    execution.LeaveRadius();
                    _encounterExecutionRepository.Update(execution);
                }
                return new EncounterUpdateResultDto { IsCompleted = false };
            }

            if (execution.WithinRadiusSinceUtc == null)
            {
                execution.EnterRadius();
                _encounterExecutionRepository.Update(execution);
            }

            var secondsInside = (DateTime.UtcNow - execution.WithinRadiusSinceUtc.Value).TotalSeconds;
            if (secondsInside >= encounter.CompletionHoldSeconds)
            {
                execution.Complete();
                UpdateParticipance((int)touristId, encounter.ExperiencePoints);
                _encounterExecutionRepository.Update(execution);
                return new EncounterUpdateResultDto
                {
                    IsCompleted = true,
                    ExperiencePointsGained = encounter.ExperiencePoints
                };
            }
            return new EncounterUpdateResultDto { IsCompleted = false };
        }

        public EncounterUpdateResultDto FailQuiz(long touristId, long encounterId)
        {
            var encounter = _encounterRepository.GetById(encounterId) as QuizEncounter;
            if (encounter == null)
                throw new ArgumentException("Encounter not found.");

            var execution = _encounterExecutionRepository.Get(touristId, encounterId);
            if (execution == null)
                throw new NotFoundException("Encounter execution not found. Activate encounter first.");

            if (execution.Status == EncounterExecutionStatus.Completed)
                throw new InvalidOperationException("Encounter already completed!");

            execution.Complete();
            _encounterExecutionRepository.Update(execution);

            return new EncounterUpdateResultDto
            {
                IsCompleted = false,
                ExperiencePointsGained = 0
            };
        }


        //For misc encounters
        public EncounterUpdateResultDto CompleteEncounter(long touristId, long encounterId)
        {
            var encounter = _encounterRepository.GetById(encounterId);
            if (encounter == null)
                throw new ArgumentException("Encounter not found.");

            var execution = _encounterExecutionRepository.Get(touristId, encounterId);
            if (execution == null)
                throw new NotFoundException("Encounter execution not found. Activate encounter first.");

            if (execution.Status == EncounterExecutionStatus.Completed)
                throw new InvalidOperationException("Encounter already completed!");

            if (encounter.Type != DomainEncounterType.Misc)
                throw new ArgumentException("Invalid encounter type for manual completion.");
            
            execution.Complete();
            UpdateParticipance((int)touristId, encounter.ExperiencePoints);
            _encounterExecutionRepository.Update(execution);

            return new EncounterUpdateResultDto
            {
                IsCompleted = true,
                ExperiencePointsGained = encounter.ExperiencePoints
            };
        }

        public EncounterUpdateResultDto SubmitQuizAnswer(long touristId, long encounterId, List<QuizAnswerSubmitDto> answerDto)
        {
            var encounter = _encounterRepository.GetQuizById(encounterId) as QuizEncounter;
            if (encounter == null)
                throw new ArgumentException("Encounter not found.");

            var execution = _encounterExecutionRepository.Get(touristId, encounterId);
            if (execution == null)
                throw new NotFoundException("Encounter execution not found. Activate encounter first.");

            if (execution.Status == EncounterExecutionStatus.Completed)
                throw new InvalidOperationException("Encounter already completed!");

            if (!encounter.IsCompletedOnTime(execution.StartedAtUtc))
                throw new InvalidOperationException("Time limit exceeded for this quiz encounter.");

            bool allCorrect = true;
            foreach (var answerDtoItem in answerDto)
            {
                if (!encounter.IsAnswerCorrect(answerDtoItem.QuestionId, answerDtoItem.SelectedAnswerId))
                {
                    allCorrect = false;
                    break;
                }
            }

            if (!allCorrect)
            {
                execution.Complete();
                _encounterExecutionRepository.Update(execution);
                return new EncounterUpdateResultDto
                {
                    IsCompleted = false,
                    ExperiencePointsGained = 0
                };
            }

            execution.Complete();
            UpdateParticipance((int)touristId, encounter.ExperiencePoints);
            _encounterExecutionRepository.Update(execution);

            return new EncounterUpdateResultDto
            {
                IsCompleted = true,
                ExperiencePointsGained = encounter.ExperiencePoints
            };
        }


        public IEnumerable<EncounterViewDto> GetByTourist(long touristId)
        {
            return _encounterRepository.GetByTourist(touristId)
                .Select(_mapper.Map<EncounterViewDto>)
                .ToList();
        }

        public bool IsEncounterCompleted(long tourPointId, long touristId)
        {
            var encounter = _encounterRepository.GetByTourPointId((int)tourPointId);
            if (encounter == null || !encounter.IsRequiredForPointCompletion)
                return true;

            return _encounterExecutionRepository.Get(touristId, encounter.Id)?
                .Status == EncounterExecutionStatus.Completed;
        }

        private static Location ToDomainLocation(LocationDto dto)
        {
            return new Location(dto.Longitude, dto.Latitude);
        }
    }
}
