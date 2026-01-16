using AutoMapper;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Encounters.API.Dtos;
using Explorer.Encounters.API.Public.Administration;

using Explorer.Encounters.Core.Domain;
using Explorer.Encounters.Core.Domain.Repositories;
using Explorer.Encounters.Core.Domain.RepositoryInterfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainEncounterType = Explorer.Encounters.Core.Domain.EncounterType;

namespace Explorer.Encounters.Core.UseCases
{
    public class EncounterService : IEncounterService
    {
        private readonly IEncounterRepository _encounterRepository;
        private readonly IHiddenLocationEncounterRepository _hiddenLocationEncounterRepository;
        private readonly IMapper _mapper;

        public EncounterService(IEncounterRepository encounterRepository, IMapper mapper, IHiddenLocationEncounterRepository hiddenLocationEncounterRepository)
        {
            _encounterRepository = encounterRepository;
            _mapper = mapper;
            _hiddenLocationEncounterRepository = hiddenLocationEncounterRepository;
        }

        public EncounterDto Create(EncounterDto dto, bool needsApproval)
        {
            var encounter = new Encounter(dto.Name, dto.Description, _mapper.Map<Location>(dto.Location), dto.ExperiencePoints, (Domain.EncounterType)dto.Type, needsApproval ? EncounterApprovalStatus.PENDING : EncounterApprovalStatus.APPROVED);
            var created = _encounterRepository.Create(encounter);
            return _mapper.Map<EncounterDto>(created);
        }

        public SocialEncounterDto CreateSocial(SocialEncounterDto dto, bool needsApproval)
        {
            var social = new SocialEncounter(dto.Name, dto.Description, _mapper.Map<Location>(dto.Location), dto.ExperiencePoints, dto.MinimumParticipants, dto.ActivationRadiusMeters, needsApproval ? EncounterApprovalStatus.PENDING : EncounterApprovalStatus.APPROVED);

            var created = _encounterRepository.Create(social) as SocialEncounter;
            return _mapper.Map<SocialEncounterDto>(created);
        }

        public SocialEncounterDto UpdateSocial(SocialEncounterDto dto, int encounterId)
        {
            var social = _encounterRepository.GetById(encounterId) as SocialEncounter;
            if (social == null)
                throw new InvalidOperationException("Encounter is not a SocialEncounter.");


            social.UpdateSocial(dto.Name, dto.Description, _mapper.Map<Location>(dto.Location), dto.ExperiencePoints, dto.MinimumParticipants, dto.ActivationRadiusMeters);
          
            var updated = _encounterRepository.Update(social) as SocialEncounter;
            return _mapper.Map<SocialEncounterDto>(updated!);

        }

        public HiddenLocationEncounterDto CreateHiddenLocation(HiddenLocationEncounterDto dto, bool needsApproval)
        {
            var hidden = new HiddenLocationEncounter(
                dto.Name,
                dto.Description,
                _mapper.Map<Location>(dto.Location),
                dto.ExperiencePoints,
                dto.ImageUrl,
                _mapper.Map<Location>(dto.PhotoPoint),
                dto.ActivationRadiusMeters, 
                needsApproval ? EncounterApprovalStatus.PENDING : EncounterApprovalStatus.APPROVED
            );

            var created = _hiddenLocationEncounterRepository.Create(hidden);
            return _mapper.Map<HiddenLocationEncounterDto>(created);
        }
        public HiddenLocationEncounterDto UpdateHiddenLocation(HiddenLocationEncounterDto dto, int encounterId)
        {
            var hidden = _hiddenLocationEncounterRepository.Get(encounterId);

            // update base fields (type stays Location)
            hidden.Update(
                dto.Name,
                dto.Description,
                _mapper.Map<Location>(dto.Location),
                dto.ExperiencePoints,
                DomainEncounterType.Location
            );

            // update hidden-specific fields
            hidden.UpdateHiddenData(
                dto.ImageUrl,
                _mapper.Map<Location>(dto.PhotoPoint),
                dto.ActivationRadiusMeters
            );

            var updated = _hiddenLocationEncounterRepository.Update(hidden);
            return _mapper.Map<HiddenLocationEncounterDto>(updated);
        }
        public void Delete(long id)
        {
            _encounterRepository.Delete(id);
        }

        public PagedResult<EncounterDto> GetPaged(int page, int pageSize)
        {
            var all = _encounterRepository.GetPaged(page, pageSize);
            var mapped = all.Results.Select(_mapper.Map<EncounterDto>).ToList();
            return new PagedResult<EncounterDto>(mapped, all.Results.Count);
        }

        public IEnumerable<EncounterDto> GetActive()
        {
            return _encounterRepository.GetActive()
                .Select(_mapper.Map<EncounterDto>);
        }

        public EncounterDto Update(EncounterUpdateDto dto, int encounterId)
        {
            var encounter = _encounterRepository.GetById(encounterId);
            encounter.Update(dto.Name, dto.Description, _mapper.Map<Location>(dto.Location), dto.ExperiencePoints, (Domain.EncounterType)dto.Type);

            return _mapper.Map<EncounterDto>(_encounterRepository.Update(encounter));
        }

        public void AddEncounterToTourPoint(long encounterId, long tourPointId, bool isRequiredForPointCompletion)
        {
            var encounter = _encounterRepository.GetById(encounterId);
            if (encounter == null)
            {
                throw new NotFoundException($"Encounter with id {encounterId} not found.");
            }
            encounter.SetTourPoint(tourPointId, isRequiredForPointCompletion);
            _encounterRepository.Update(encounter);
        }

        public List<EncounterDto> GetByTourPointIds(IEnumerable<int> tourPointIds)
        {
            var encounters = _encounterRepository.GetByTourPointIds(tourPointIds);
            return encounters.Select(_mapper.Map<EncounterDto>).ToList();
        }

        public void Publish(int id)
        {
            var encounter = _encounterRepository.GetById(id);
            if (encounter == null)
            {
                throw new NotFoundException($"Encounter with id {id} not found.");
            }   
            encounter.Activate();
            _encounterRepository.Update(encounter);
        }

        public void Archive(int id)
        {
            var encounter = _encounterRepository.GetById(id);
            if (encounter == null)
            {
                throw new NotFoundException($"Encounter with id {id} not found.");
            }
            encounter.Archive();
            _encounterRepository.Update(encounter);
        }

        public void Approve(long id)
        {
            var encounter = _encounterRepository.GetById(id);
            if (encounter == null)
            {
                throw new NotFoundException($"Encounter with id {id} not found.");
            }
            encounter.Approve();
            _encounterRepository.Update(encounter);
        }

        public void Decline(long id)
        {
            var encounter = _encounterRepository.GetById(id);
            if (encounter == null)
            {
                throw new NotFoundException($"Encounter with id {id} not found.");
            }
            encounter.Decline();
            _encounterRepository.Update(encounter);
        }

        public IEnumerable<EncounterViewDto> GetPendingApproval()
        {
            return _encounterRepository.GetPendingEncounters()
                .Select(_mapper.Map<EncounterViewDto>)
                .ToList();
        }
    }
}
