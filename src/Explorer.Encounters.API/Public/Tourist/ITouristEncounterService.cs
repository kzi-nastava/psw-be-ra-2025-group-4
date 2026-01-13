using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Explorer.Encounters.API.Dtos;
using Explorer.BuildingBlocks.Core.UseCases;

namespace Explorer.Encounters.API.Public.Tourist
{
    public interface ITouristEncounterService
    {
        EncounterDto StartEncounter(long touristId, long encounterId);
        EncounterDto CompleteEncounter(long touristId, long encounterId);
    }
}
