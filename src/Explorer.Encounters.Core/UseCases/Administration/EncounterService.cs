using AutoMapper;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Encounters.API.Dtos;
using Explorer.Encounters.API.Public.Administration;
using Explorer.Encounters.Core.Domain;
using Explorer.Encounters.Core.Domain.RepositoryInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Encounters.Core.UseCases.Administration
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

        public EncounterDto Create(EncounterDto dto)
        {
            var encounter = new Encounter(dto.Name, dto.Description, _mapper.Map<Location>(dto.Location), dto.ExperiencePoints, (Domain.EncounterType)dto.Type);
            var created = _encounterRepository.Create(encounter);
            return _mapper.Map<EncounterDto>(created);
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

        public EncounterDto Update(EncounterUpdateDto dto)
        {
            var encounter = _encounterRepository.GetById(dto.Id);
            encounter.Update(dto.Name, dto.Description, _mapper.Map<Location>(dto.Location), dto.ExperiencePoints, (Domain.EncounterType)dto.Type);

            return _mapper.Map<EncounterDto>(_encounterRepository.Update(encounter));
        }
    }
}
