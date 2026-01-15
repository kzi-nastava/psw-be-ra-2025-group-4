using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Encounters.Core.Domain.RepositoryInterfaces
{
    public interface IEncounterParticipantRepository
    {
        EncounterParticipant? Get(long userId);
        EncounterParticipant Add(EncounterParticipant progress);
        EncounterParticipant Update(EncounterParticipant progress);
        IEnumerable<EncounterParticipant> GetAll();
    }
}
