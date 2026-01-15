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
        void CompleteEncounter(long touristId, long encounterId);
        void UpdateLocation (long touristId, long encounterId, LocationDto touristLocation);
        List<EncounterViewDto> GetByTourPoint(long touristId, int tourPointId, LocationDto touristLocation);
        int UpdateTouristLocation(long encounterId, long touristId, double lat, double lng);

    }
}
