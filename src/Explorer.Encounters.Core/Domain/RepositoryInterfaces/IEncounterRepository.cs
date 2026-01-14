using Explorer.BuildingBlocks.Core.UseCases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Encounters.Core.Domain.RepositoryInterfaces
{
    public interface IEncounterRepository
    {
        Encounter Create(Encounter encounter);
        Encounter? GetById(long id);
        PagedResult<Encounter> GetPaged(int page, int pageSize);
        List<Encounter> GetByTourPointIds(IEnumerable<int> tourPointIds);
        public IEnumerable<Encounter> GetActive();
        Encounter Update(Encounter encounter);
        void Delete(long id);
        bool ExistsByTourPoint(long tourPointId);
    }
}
