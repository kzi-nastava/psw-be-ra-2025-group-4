using AutoMapper;
using Explorer.Encounters.API.Dtos;
using Explorer.Encounters.API.Public.Tourist;
using Explorer.Encounters.Core.Domain.RepositoryInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Encounters.Core.UseCases.Tourist
{
    public class EncounterParticipantService : IEncounterParticipantService
    {
        private readonly IEncounterParticipantRepository _encounterParticipantRepository;
        private readonly IMapper _mapper;

        public EncounterParticipantService(IEncounterParticipantRepository encounterParticipantRepository, IMapper mapper)
        {
            _encounterParticipantRepository = encounterParticipantRepository;
            _mapper = mapper;
        }

        public EncounterParticipantDto? Get(long touristId)
        {
            return _mapper.Map<EncounterParticipantDto>(_encounterParticipantRepository.Get(touristId));
        }

        public int GetExperience(long touristId)
        {
            return _encounterParticipantRepository.Get(touristId)?.ExperiencePoints ?? 0;
        }

        public int GetLevel(long touristId)
        {
            return _encounterParticipantRepository.Get(touristId)?.Level ?? 0;
        }
    }
}
