using AutoMapper;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Encounters.API.Dtos;
using Explorer.Encounters.API.Public.Administration;
using Explorer.Encounters.Core.Domain;
using Explorer.Encounters.Core.Domain.RepositoryInterfaces;

namespace Explorer.Encounters.Core.UseCases
{
    public class EncounterService : IEncounterService
    {
        private readonly IEncounterRepository _encounterRepository;
        private readonly IMapper _mapper;

        public EncounterService(IEncounterRepository encounterRepository, IMapper mapper)
        {
            _encounterRepository = encounterRepository;
            _mapper = mapper;
        }

        public EncounterDto Create(EncounterDto dto, bool needsApproval)
        {
            var encounter = new Encounter(
                dto.Name,
                dto.Description,
                _mapper.Map<Location>(dto.Location),
                dto.ExperiencePoints,
                (Domain.EncounterType)dto.Type,
                needsApproval ? EncounterApprovalStatus.PENDING : EncounterApprovalStatus.APPROVED
            );

            var created = _encounterRepository.Create(encounter);
            return _mapper.Map<EncounterDto>(created);
        }

        public SocialEncounterDto CreateSocial(SocialEncounterDto dto, bool needsApproval)
        {
            var social = new SocialEncounter(
                dto.Name,
                dto.Description,
                _mapper.Map<Location>(dto.Location),
                dto.ExperiencePoints,
                dto.MinimumParticipants,
                dto.ActivationRadiusMeters,
                needsApproval ? EncounterApprovalStatus.PENDING : EncounterApprovalStatus.APPROVED
            );

            var created = _encounterRepository.Create(social) as SocialEncounter;
            return _mapper.Map<SocialEncounterDto>(created);
        }

        public HiddenLocationEncounterDto CreateHiddenLocation(HiddenLocationEncounterDto dto, bool needsApproval)
        {
            var hidden = new HiddenLocationEncounter(dto.Name,
                dto.Description,
                _mapper.Map<Location>(dto.Location),
                dto.ExperiencePoints,
                dto.ImageUrl,
                _mapper.Map<Location>(dto.PhotoPoint),
                dto.ActivationRadiusMeters,
                needsApproval ? EncounterApprovalStatus.PENDING : EncounterApprovalStatus.APPROVED
            );

            var created = _encounterRepository.Create(hidden) as HiddenLocationEncounter;
            return _mapper.Map<HiddenLocationEncounterDto>(created);
        }

        public QuizEncounterDto CreateQuiz(QuizEncounterDto dto, bool needsApproval)
        {
            var quiz = new QuizEncounter(dto.Name,
                dto.Description,
                _mapper.Map<Location>(dto.Location),
                dto.ExperiencePoints,
                needsApproval ? EncounterApprovalStatus.PENDING : EncounterApprovalStatus.APPROVED,
                dto.Questions.Select(question => _mapper.Map<QuizQuestion>(question)).ToList(),
                dto.TimeLimit);

            var created = _encounterRepository.Create(quiz) as QuizEncounter;
            return _mapper.Map<QuizEncounterDto>(created);
        }

        public QuizEncounterDto UpdateQuiz(QuizEncounterDto dto, int encounterId)
        {
            var quiz = GetEncounterOrThrow<QuizEncounter>(encounterId);

            quiz.UpdateQuiz(
                dto.Name,
                dto.Description,
                _mapper.Map<Location>(dto.Location),
                dto.ExperiencePoints,
                dto.Questions.Select(question => _mapper.Map<QuizQuestion>(question)).ToList(),
                dto.TimeLimit);

            return _mapper.Map<QuizEncounterDto>(_encounterRepository.Update(quiz));

        }

        public EncounterDto Update(EncounterUpdateDto dto, int encounterId)
        {
            var encounter = GetEncounterOrThrow(encounterId);

            encounter.Update(dto.Name,dto.Description,_mapper.Map<Location>(dto.Location),dto.ExperiencePoints,(Domain.EncounterType)dto.Type);

            return _mapper.Map<EncounterDto>(_encounterRepository.Update(encounter));
        }

        public SocialEncounterDto UpdateSocial(SocialEncounterDto dto, int encounterId)
        {
            var social = GetEncounterOrThrow<SocialEncounter>(encounterId);

            social.UpdateSocial(
                dto.Name,
                dto.Description,
                _mapper.Map<Location>(dto.Location),
                dto.ExperiencePoints,
                dto.MinimumParticipants,
                dto.ActivationRadiusMeters
            );

            return _mapper.Map<SocialEncounterDto>(_encounterRepository.Update(social));
        }

        public HiddenLocationEncounterDto UpdateHiddenLocation(HiddenLocationEncounterDto dto, int encounterId)
        {
            var hidden = GetEncounterOrThrow<HiddenLocationEncounter>(encounterId);

            hidden.Update(
                dto.Name,
                dto.Description,
                _mapper.Map<Location>(dto.Location),
                dto.ExperiencePoints,
                Domain.EncounterType.Location
            );

            hidden.UpdateHiddenData(
                dto.ImageUrl,
                _mapper.Map<Location>(dto.PhotoPoint),
                dto.ActivationRadiusMeters
            );

            return _mapper.Map<HiddenLocationEncounterDto>(_encounterRepository.Update(hidden));
        }

        public void Delete(long id)
        {
            var encounter = _encounterRepository.GetById(id);
            if (encounter == null)
                throw new NotFoundException($"Encounter with id {id} not found."); 
            _encounterRepository.Delete(id);
        }

        public PagedResult<EncounterViewDto> GetPaged(int page, int pageSize)
        {
            var paged = _encounterRepository.GetPaged(page, pageSize);

            if (paged == null || paged.Results == null)
            {
                return new PagedResult<EncounterViewDto>(new List<EncounterViewDto>(), 0);
            }

            var mapped = paged.Results.Select(_mapper.Map<EncounterViewDto>).ToList();

            return new PagedResult<EncounterViewDto>(mapped, paged.TotalCount);
        }

        public IEnumerable<EncounterDto> GetActive()
        {
            return _encounterRepository
                .GetActive()
                .Select(_mapper.Map<EncounterDto>);
        }

        public List<EncounterViewDto> GetByTourPointIds(IEnumerable<int> tourPointIds)
        {
            return _encounterRepository
                .GetByTourPointIds(tourPointIds)
                .Select(_mapper.Map<EncounterViewDto>)
                .ToList();
        }

        public IEnumerable<EncounterViewDto> GetPendingApproval()
        {
            return _encounterRepository
                .GetPendingEncounters()
                .Select(_mapper.Map<EncounterViewDto>)
                .ToList();
        }

        public void Publish(int id)
        {
            var encounter = GetEncounterOrThrow(id);
            encounter.Activate();
            _encounterRepository.Update(encounter);
        }

        public void Archive(int id)
        {
            var encounter = GetEncounterOrThrow(id);
            encounter.Archive();
            _encounterRepository.Update(encounter);
        }

        public void Approve(long id)
        {
            var encounter = GetEncounterOrThrow(id);
            encounter.Approve();
            _encounterRepository.Update(encounter);
        }

        public void Decline(long id)
        {
            var encounter = GetEncounterOrThrow(id);
            encounter.Decline();
            _encounterRepository.Update(encounter);
        }

        public void AddEncounterToTourPoint(long encounterId, long tourPointId, bool isRequiredForPointCompletion)
        {
            var encounter = GetEncounterOrThrow(encounterId);
            encounter.SetTourPoint(tourPointId, isRequiredForPointCompletion);
            _encounterRepository.Update(encounter);
        }

        private Encounter GetEncounterOrThrow(long id)
        {
            return _encounterRepository.GetById(id)
                ?? throw new NotFoundException($"Encounter with id {id} not found.");
        }

        private TEncounter GetEncounterOrThrow<TEncounter>(long id)
            where TEncounter : Encounter
        {
            var encounter = GetEncounterOrThrow(id);

            if (encounter is not TEncounter typed)
                throw new InvalidOperationException(
                    $"Encounter {id} is not of type {typeof(TEncounter).Name}.");

            return typed;
        }
    }
}
