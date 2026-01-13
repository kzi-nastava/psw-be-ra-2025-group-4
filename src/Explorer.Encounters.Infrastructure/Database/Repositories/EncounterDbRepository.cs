using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.BuildingBlocks.Infrastructure.Database;
using Explorer.Encounters.Core.Domain;
using Explorer.Encounters.Core.Domain.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Encounters.Infrastructure.Database.Repositories
{
    public class EncounterDbRepository : IEncounterRepository
    {
        protected readonly EncountersContext DbContext;
        private readonly DbSet<Encounter> _dbSet;

        public EncounterDbRepository(EncountersContext dbContext)
        {
            DbContext = dbContext;
            _dbSet = DbContext.Set<Encounter>();
        }

        public Encounter Create(Encounter encounter)
        {
            _dbSet.Add(encounter);
            DbContext.SaveChanges();
            return encounter;
        }

        public void Delete(long id)
        {
            var entity = GetById(id);
            if (entity == null)
                throw new NotFoundException("Encounter not found: " + id);

            _dbSet.Remove(entity);
            DbContext.SaveChanges();
        }

        public Encounter? GetById(long id)
        {
            var entity = _dbSet
                .FirstOrDefault(t => t.Id == id);

            if (entity == null)
                throw new NotFoundException("Encounter not found: " + id);

            return entity;
        }

        public PagedResult<Encounter> GetPaged(int pageNumber, int pageSize)
        {
            var task = _dbSet
                .GetPagedById(pageSize, pageNumber);
            task.Wait();
            return task.Result;
        }

        public IEnumerable<Encounter> GetActive()
        {
            return _dbSet
                .Where(e => e.Status == EncounterStatus.Active)
                .ToList();
        }

        public Encounter Update(Encounter encounter)
        {
            try
            {
                DbContext.Update(encounter);
                DbContext.SaveChanges();
            }
            catch (DbUpdateException e)
            {
                throw new NotFoundException(e.Message);
            }
            return encounter;
        }

        public bool ExistsByTourPoint(long tourPointId)
        {
            return _dbSet
                .Any(e => e.TourPointId == tourPointId);
        }

        public List<Encounter> GetByTourPointIds(IEnumerable<int> tourPointIds)
        {
            return _dbSet
                .Where(e => e.TourPointId.HasValue &&
                            tourPointIds.Contains((int)e.TourPointId.Value))
                .ToList();
        }
    }
}
