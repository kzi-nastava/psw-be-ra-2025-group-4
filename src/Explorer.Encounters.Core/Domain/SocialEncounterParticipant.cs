using Explorer.BuildingBlocks.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Encounters.Core.Domain
{
    public class SocialEncounterParticipant : Entity
    {        
        public long SocialEncounterId { get; private set; }
        public SocialEncounter SocialEncounter { get; private set; }
        public long TouristId { get; private set; }
        public DateTime LastSeenAt { get; private set; }
        
        private SocialEncounterParticipant()
        {

        }

        public SocialEncounterParticipant(long socialEncounterId, long touristId, DateTime lastSeenAt)
        {
            SocialEncounterId = socialEncounterId;
            TouristId = touristId;
            LastSeenAt = lastSeenAt;
        }

        public void Update(long socialEncounterId, long touristId, DateTime lastSeenAt)
        {

        }

        public void SetLastSeenAt(DateTime lastSeenAt)
        {
            LastSeenAt = lastSeenAt;
        }
    }
}
