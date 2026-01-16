using Explorer.Encounters.API.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Encounters.API.Public.Tourist
{
    public interface IEncounterParticipantService
    {
        int GetLevel(long touristId);
        int GetExperience(long touristId);
        EncounterParticipantDto Get(long touristId);
    }
}
