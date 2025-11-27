using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;
using Explorer.Tours.Core.Domain;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.BuildingBlocks.Infrastructure.Database;
using Explorer.BuildingBlocks.Core.Exceptions;

namespace Explorer.Tours.Infrastructure.Database.Repositories
{
    public class FacilityDbRepository : IFacilityRepository
    {
        protected readonly ToursContext DbContext;
        private readonly DbSet<Facility> _dbSet;

        public FacilityDbRepository(ToursContext dbContext)
        {
            DbContext = dbContext;
            _dbSet = DbContext.Set<Facility>();
        }

        public PagedResult<Facility> GetPaged(int page, int pageSize)
        {
            var task = _dbSet.GetPagedById(page, pageSize);
            task.Wait();
            return task.Result;
        }

        public Facility Get(long id)
        {
            var entity = _dbSet.Find(id);
            if(entity == null) throw new NotFoundException("Not found: " + id);
            return entity;
        }

        public Facility Create(Facility entity)
        {
            _dbSet.Add(entity);
            DbContext.SaveChanges();
            return entity;
        }

        public Facility Update(Facility entity)
        {
            DbContext.SaveChanges();
            return entity;
        }

        public void Delete(long id)
        {
            var entity = Get(id);
            _dbSet.Remove(entity);
            DbContext.SaveChanges();
        }
    }
}