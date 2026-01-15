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

        public EncounterParticipantService(IEncounterParticipantRepository encounterParticipantRepository)
        {
            _encounterParticipantRepository = encounterParticipantRepository;
        }

        public int GetLevel(long touristId)
        {
            return _encounterParticipantRepository.Get(touristId)?.Level ?? 0;
        }
    }
}
