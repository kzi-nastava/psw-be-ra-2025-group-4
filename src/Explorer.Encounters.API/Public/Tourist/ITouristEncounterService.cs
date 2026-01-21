using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Explorer.Encounters.API.Dtos;
using Explorer.BuildingBlocks.Core.UseCases;
using System.Collections.Generic;

namespace Explorer.Encounters.API.Public.Tourist
{
    public interface ITouristEncounterService
    {
        void StartEncounter(long touristId, long encounterId);
        List<EncounterViewDto> GetByTourPoint(long touristId, int tourPointId, LocationDto touristLocation);
        EncounterUpdateResultDto UpdateSocialEncounter(long touristId, long encounterId, LocationDto touristLocation);
        EncounterUpdateResultDto UpdateLocationHiddenEncounter(long touristId, long encounterId, LocationDto touristLocation);
        EncounterUpdateResultDto CompleteEncounter(long touristId, long encounterId);
        IEnumerable<EncounterViewDto> GetByTourist(long touristId);
        bool IsEncounterCompleted(long tourPointId, long touristId);
    }
}
