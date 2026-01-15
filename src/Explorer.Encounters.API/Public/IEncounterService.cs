using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Encounters.API.Dtos;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Encounters.API.Public.Administration
{
    public interface IEncounterService
    {
        PagedResult<EncounterDto> GetPaged(int page, int pageSize);

        public List<EncounterDto> GetByTourPointIds(IEnumerable<int> tourPointIds);
        IEnumerable<EncounterDto> GetActive();
        EncounterDto Create(EncounterDto dto, bool needsApproval);
        HiddenLocationEncounterDto CreateHiddenLocation(HiddenLocationEncounterDto dto, bool needsApproval);
        HiddenLocationEncounterDto UpdateHiddenLocation(HiddenLocationEncounterDto dto, int encounterId);

        SocialEncounterDto CreateSocial(SocialEncounterDto dto, bool needsApproval);
        SocialEncounterDto UpdateSocial(SocialEncounterDto dto, int encounterId);

        EncounterDto Update(EncounterUpdateDto dto, int encounterId);
        void Publish(int id);
        void Archive(int id);
        void Delete(long id);
        void AddEncounterToTourPoint(long encounterId, long tourPointId, bool isRequiredForPointCompletion);
        void Approve(long id);
        void Decline(long id);
        IEnumerable<EncounterViewDto> GetPendingApproval();
    }
}
