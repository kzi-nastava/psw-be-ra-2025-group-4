using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Encounters.Core.Domain.RepositoryInterfaces
{
    public interface ISocialEncounterParticipantRepository
    {
        void Add(SocialEncounterParticipant participant);
        SocialEncounterParticipant? Get(long encounterId, long touristId);
        void Update(SocialEncounterParticipant participant);
        void Remove(SocialEncounterParticipant participant);
        void RemoveStale(long encounterId, TimeSpan threshold, DateTime referenceTime);
        int CountActive(long encounterId, DateTime referenceTime);
        IEnumerable<SocialEncounterParticipant> GetActive(long encounterId, DateTime referenceTime);
    }
}
