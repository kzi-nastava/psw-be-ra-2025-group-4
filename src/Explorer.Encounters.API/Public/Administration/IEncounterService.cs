using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Encounters.API.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Encounters.API.Public.Administration
{
    public interface IEncounterService
    {
        PagedResult<EncounterDto> GetPaged(int page, int pageSize);
        IEnumerable<EncounterDto> GetActive();
        EncounterDto Create(EncounterDto dto);
        EncounterDto Update(EncounterUpdateDto dto);
        void Delete(long id);
    }
}
